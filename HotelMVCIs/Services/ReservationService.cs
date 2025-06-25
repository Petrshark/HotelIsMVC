using HotelMVCIs.Data;
using HotelMVCIs.DTOs;
using HotelMVCIs.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace HotelMVCIs.Services
{
    public class ReservationService
    {
        private readonly HotelMVCIsDbContext _context;
        private readonly PaymentService _paymentService;

        public ReservationService(HotelMVCIsDbContext context, PaymentService paymentService)
        {
            _context = context;
            _paymentService = paymentService;
        }

        public async Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut, int? reservationIdToExclude = null)
        {
            var query = _context.Reservations
                .Where(r => r.RoomId == roomId &&
                            r.Status != ReservationStatus.Cancelled &&
                            r.CheckInDate < checkOut &&
                            r.CheckOutDate > checkIn);

            if (reservationIdToExclude.HasValue)
            {
                query = query.Where(r => r.Id != reservationIdToExclude.Value);
            }
            return !await query.AnyAsync();
        }

        // ZMĚNA: GetAllAsync bude nyní vracet IEnumerable<ReservationDTO> a naplní nové vlastnosti
        public async Task<IEnumerable<ReservationDTO>> GetAllAsync() // <--- ZMĚNA NÁVRATOVÉHO TYPU
        {
            // Načteme všechny rezervace s hosty a pokoji
            var reservations = await _context.Reservations
                .Include(r => r.Guest)
                .Include(r => r.Room)
                    .ThenInclude(room => room.RoomType)
                .OrderByDescending(r => r.CheckInDate)
                .ToListAsync();

            var reservationDTOs = new List<ReservationDTO>();

            foreach (var reservation in reservations)
            {
                // Pro každou rezervaci zjistíme celkovou zaplacenou částku
                var totalPaid = await _paymentService.GetTotalPaidForReservationAsync(reservation.Id);
                var remaining = reservation.TotalPrice - totalPaid;

                reservationDTOs.Add(new ReservationDTO
                {
                    Id = reservation.Id,
                    GuestId = reservation.GuestId, // Ponecháme ID
                    RoomId = reservation.RoomId,   // Ponecháme ID

                    // Nové vlastnosti pro zobrazení
                    GuestFullName = $"{reservation.Guest.FirstName} {reservation.Guest.LastName}",
                    RoomNumber = reservation.Room.RoomNumber,
                    RoomTypeName = reservation.Room.RoomType.Name,

                    CheckInDate = reservation.CheckInDate,
                    CheckOutDate = reservation.CheckOutDate,
                    NumberOfGuests = reservation.NumberOfGuests,
                    TotalPrice = reservation.TotalPrice, // Nyní mapujeme TotalPrice
                    Status = reservation.Status,
                    RemainingBalance = remaining // <-- Vypočítaná hodnota
                });
            }

            return reservationDTOs;
        }

        public async Task<ReservationDTO> GetByIdForEditAsync(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null) return null;

            // Zde nenačítáme RemainingBalance ani FullName, protože to není pro Edit formulář potřeba.
            // Pokud byste to v Edit formuláři potřebovali, museli bychom to zde také načíst/vypočítat.
            return new ReservationDTO
            {
                Id = reservation.Id,
                GuestId = reservation.GuestId,
                RoomId = reservation.RoomId,
                CheckInDate = reservation.CheckInDate,
                CheckOutDate = reservation.CheckOutDate,
                NumberOfGuests = reservation.NumberOfGuests,
                // TotalPrice a Status jsou již v původním modelu Reservation
                // RemainingBalance není pro editaci potřeba, je to jen pro zobrazení
                Status = reservation.Status
            };
        }

        public async Task<Reservation> GetByIdForDeleteAsync(int id)
        {
            return await _context.Reservations
               .Include(r => r.Guest)
               .Include(r => r.Room)
                   .ThenInclude(room => room.RoomType)
               .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task CreateAsync(ReservationDTO dto)
        {
            var room = await _context.Rooms.FindAsync(dto.RoomId);
            if (room == null) return;

            var nights = (dto.CheckOutDate - dto.CheckInDate).Days;
            if (nights <= 0) nights = 1;

            var totalPrice = nights * room.PricePerNight;

            var reservation = new Reservation
            {
                GuestId = dto.GuestId,
                RoomId = dto.RoomId,
                CheckInDate = dto.CheckInDate,
                CheckOutDate = dto.CheckOutDate,
                NumberOfGuests = dto.NumberOfGuests,
                TotalPrice = totalPrice,
                Status = dto.Status
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ReservationDTO dto)
        {
            var reservation = await _context.Reservations.FindAsync(dto.Id);
            var room = await _context.Rooms.FindAsync(dto.RoomId);
            if (reservation == null || room == null) return;

            var nights = (dto.CheckOutDate - dto.CheckInDate).Days;
            if (nights <= 0) nights = 1;

            var totalPrice = nights * room.PricePerNight;

            reservation.GuestId = dto.GuestId;
            reservation.RoomId = dto.RoomId;
            reservation.CheckInDate = dto.CheckInDate;
            reservation.CheckOutDate = dto.CheckOutDate;
            reservation.NumberOfGuests = dto.NumberOfGuests;
            reservation.TotalPrice = totalPrice; // Aktualizujeme TotalPrice
            reservation.Status = dto.Status;

            _context.Update(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<DateTime?> GetEarliestReservationDateAsync()
        {
            var earliestReservation = await _context.Reservations
                .Where(r => r.Status != ReservationStatus.Cancelled)
                .OrderBy(r => r.CheckInDate)
                .FirstOrDefaultAsync();

            return earliestReservation?.CheckInDate;
        }
    }
}