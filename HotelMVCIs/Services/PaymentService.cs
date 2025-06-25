using HotelMVCIs.Data;
using HotelMVCIs.DTOs;
using HotelMVCIs.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace HotelMVCIs.Services
{
    public class PaymentService
    {
        private readonly HotelMVCIsDbContext _context;

        public PaymentService(HotelMVCIsDbContext context)
        {
            _context = context;
        }

        // ZMĚNA: GetAllAsync bude nyní vracet IEnumerable<PaymentDTO> a naplní nové vlastnosti
        public async Task<IEnumerable<PaymentDTO>> GetAllAsync() // <--- Návratový typ je stále PaymentDTO
        {
            // Načteme všechny platby s jejich souvisejícími rezervacemi, hosty a pokoji
            var payments = await _context.Payments
                .Include(p => p.Reservation)
                    .ThenInclude(r => r.Guest)
                .Include(p => p.Reservation)
                    .ThenInclude(r => r.Room)
                .OrderByDescending(p => p.PaymentDate)
                .ThenByDescending(p => p.Id)
                .ToListAsync();

            var paymentDTOs = new List<PaymentDTO>();

            foreach (var payment in payments)
            {
                // Pro každou rezervaci (ke které se platba vztahuje) zjistíme celkovou zaplacenou částku
                var totalPaidForReservation = await GetTotalPaidForReservationAsync(payment.ReservationId);
                var remainingBalanceForReservation = payment.Reservation.TotalPrice - totalPaidForReservation;

                paymentDTOs.Add(new PaymentDTO
                {
                    Id = payment.Id,
                    ReservationId = payment.ReservationId,

                    // --- NAPLNĚNÍ NOVÝCH VLASTNOSTÍ V DTO ---
                    ReservationDisplay = $"#{payment.Reservation.Id} - Pokoj: {payment.Reservation.Room.RoomNumber} ({payment.Reservation.Guest.FirstName} {payment.Reservation.Guest.LastName})",
                    ReservationTotalPrice = payment.Reservation.TotalPrice,
                    ReservationTotalPaid = totalPaidForReservation,
                    ReservationRemainingBalance = remainingBalanceForReservation,
                    // -----------------------------------------

                    Amount = payment.Amount,
                    PaymentDate = payment.PaymentDate,
                    PaymentMethod = payment.PaymentMethod,
                    Notes = payment.Notes
                });
            }

            return paymentDTOs;
        }

        public async Task<PaymentDTO> GetByIdAsync(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null) return null;

            // Zde nenačítáme ReservationDisplay, TotalPrice, TotalPaid nebo RemainingBalance,
            // protože pro formuláře Edit/Create to obvykle není nutné a načítá se to dynamicky
            // (např. TotalPrice pro validaci, RemainingBalance se přepočítává v Controlleru Create GET)
            return new PaymentDTO
            {
                Id = payment.Id,
                ReservationId = payment.ReservationId,
                Amount = payment.Amount,
                PaymentDate = payment.PaymentDate,
                PaymentMethod = payment.PaymentMethod,
                Notes = payment.Notes
            };
        }

        public async Task CreateAsync(PaymentDTO dto)
        {
            var payment = new Payment
            {
                ReservationId = dto.ReservationId,
                Amount = dto.Amount,
                PaymentDate = dto.PaymentDate,
                PaymentMethod = dto.PaymentMethod, // <--- Bez změny, typ se shodne
                Notes = dto.Notes
            };
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PaymentDTO dto)
        {
            var payment = await _context.Payments.FindAsync(dto.Id);
            if (payment != null)
            {
                payment.ReservationId = dto.ReservationId;
                payment.Amount = dto.Amount;
                payment.PaymentDate = dto.PaymentDate;
                payment.PaymentMethod = dto.PaymentMethod; // <--- Bez změny, typ se shodne
                payment.Notes = dto.Notes;

                _context.Update(payment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment != null)
            {
                _context.Payments.Remove(payment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Payments.AnyAsync(e => e.Id == id);
        }

        public async Task<Payment> GetPaymentForDeleteAsync(int id)
        {
            return await _context.Payments
                .Include(p => p.Reservation)
                    .ThenInclude(r => r.Guest)
                .Include(p => p.Reservation)
                    .ThenInclude(r => r.Room)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<SelectListItem>> GetReservationsForDropdownAsync()
        {
            return await _context.Reservations
                .Include(r => r.Guest)
                .Include(r => r.Room)
                .Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = $"Rezervace #{r.Id} - Pokoj: {r.Room.RoomNumber} - Host: {r.Guest.FirstName} {r.Guest.LastName} ({r.CheckInDate.ToShortDateString()} - {r.CheckOutDate.ToShortDateString()}) - Celková cena: {r.TotalPrice.ToString("C", new CultureInfo("cs-CZ"))}"
                })
                .OrderByDescending(item => item.Value)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalPaidForReservationAsync(int reservationId)
        {
            return await _context.Payments
                .Where(p => p.ReservationId == reservationId)
                .SumAsync(p => p.Amount);
        }

        public async Task<decimal> GetRemainingBalanceForReservationAsync(int reservationId)
        {
            var reservation = await _context.Reservations.FindAsync(reservationId);
            if (reservation == null) return 0;

            var totalPaid = await GetTotalPaidForReservationAsync(reservationId);
            return reservation.TotalPrice - totalPaid;
        }
    }
}