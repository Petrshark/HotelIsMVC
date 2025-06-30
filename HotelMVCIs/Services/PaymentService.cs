using HotelMVCIs.Data;
using HotelMVCIs.DTOs;
using HotelMVCIs.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace HotelMVCIs.Services
{
    public class PaymentService
    {
        private readonly HotelMVCIsDbContext _context;

        public PaymentService(HotelMVCIsDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PaymentDTO>> GetAllAsync()
        {
            var payments = await _context.Payments
                .Include(p => p.Reservation).ThenInclude(r => r.Guest)
                .Include(p => p.Reservation).ThenInclude(r => r.Room)
                .Include(p => p.Reservation).ThenInclude(r => r.ReservationItems)
                .OrderByDescending(p => p.PaymentDate)
                .ThenByDescending(p => p.Id)
                .ToListAsync();

            var relevantReservationIds = payments.Select(p => p.ReservationId).Distinct().ToList();

            var totalPaymentsByReservation = await _context.Payments
                .Where(p => relevantReservationIds.Contains(p.ReservationId))
                .GroupBy(p => p.ReservationId)
                .Select(g => new { ReservationId = g.Key, TotalPaid = g.Sum(p => p.Amount) })
                .ToDictionaryAsync(x => x.ReservationId, x => x.TotalPaid);

            var paymentDTOs = payments.Select(payment =>
            {
                totalPaymentsByReservation.TryGetValue(payment.ReservationId, out var totalPaidForReservation);
                var servicesPrice = payment.Reservation.ReservationItems.Sum(ri => ri.Quantity * ri.PricePerItem);
                var grandTotal = payment.Reservation.TotalPrice + servicesPrice;

                return new PaymentDTO
                {
                    Id = payment.Id,
                    ReservationId = payment.ReservationId,
                    ReservationDisplay = $"#{payment.Reservation.Id} - Pokoj: {payment.Reservation.Room.RoomNumber} ({payment.Reservation.Guest.FirstName} {payment.Reservation.Guest.LastName})",
                    ReservationTotalPrice = grandTotal,
                    ReservationTotalPaid = totalPaidForReservation,
                    ReservationRemainingBalance = grandTotal - totalPaidForReservation,
                    Amount = payment.Amount,
                    PaymentDate = payment.PaymentDate,
                    PaymentMethod = payment.PaymentMethod,
                    Notes = payment.Notes
                };
            }).ToList();

            return paymentDTOs;
        }

        public async Task<PaymentDTO?> GetByIdAsync(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null) return null;

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
                PaymentMethod = dto.PaymentMethod,
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
                payment.PaymentMethod = dto.PaymentMethod;
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

        public async Task<Payment?> GetPaymentForDeleteAsync(int id)
        {
            return await _context.Payments
                .Include(p => p.Reservation).ThenInclude(r => r.Guest)
                .Include(p => p.Reservation).ThenInclude(r => r.Room)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<SelectListItem>> GetReservationsForDropdownAsync()
        {
            var reservations = await _context.Reservations
                .Where(r => r.Status != ReservationStatus.Cancelled)
                .Include(r => r.Guest)
                .Include(r => r.Room)
                .Include(r => r.ReservationItems)
                .OrderByDescending(r => r.Id)
                .ToListAsync();

            var activeReservationIds = reservations.Select(r => r.Id).ToList();

            var totalPaymentsByReservation = await _context.Payments
                .Where(p => activeReservationIds.Contains(p.ReservationId))
                .GroupBy(p => p.ReservationId)
                .Select(g => new { ReservationId = g.Key, TotalPaid = g.Sum(p => p.Amount) })
                .ToDictionaryAsync(x => x.ReservationId, x => x.TotalPaid);

            var dropdownItems = new List<SelectListItem>();
            var culture = new CultureInfo("cs-CZ");

            foreach (var reservation in reservations)
            {
                var servicesPrice = reservation.ReservationItems.Sum(ri => ri.Quantity * ri.PricePerItem);
                var grandTotal = reservation.TotalPrice + servicesPrice;
                totalPaymentsByReservation.TryGetValue(reservation.Id, out var totalPaid);
                var remainingBalance = grandTotal - totalPaid;
                string text = $"#{reservation.Id} - {reservation.Guest.LastName}, {reservation.Room.RoomNumber} ({reservation.CheckInDate:dd.MM}) - zbývá {remainingBalance.ToString("C", culture)}";

                dropdownItems.Add(new SelectListItem
                {
                    Value = reservation.Id.ToString(),
                    Text = text,
                    Disabled = reservation.Status == ReservationStatus.Cancelled
                });
            }

            return dropdownItems;
        }

        public async Task<decimal> GetTotalPaidForReservationAsync(int reservationId)
        {
            return await _context.Payments
                .Where(p => p.ReservationId == reservationId)
                .SumAsync(p => p.Amount);
        }

        public async Task<PaymentReportDTO> GetPaymentReportAsync(DateTime startDate, DateTime endDate)
        {
            var inclusiveEndDate = endDate.AddDays(1);

            var entries = await _context.Payments
                .Where(p => p.PaymentDate >= startDate && p.PaymentDate < inclusiveEndDate)
                .GroupBy(p => new { Date = p.PaymentDate.Date, p.PaymentMethod })
                .Select(g => new ReportEntryDTO
                {
                    Date = g.Key.Date,
                    PaymentMethod = g.Key.PaymentMethod,
                    TotalAmount = g.Sum(p => p.Amount)
                })
                .OrderBy(e => e.Date)
                .ThenBy(e => e.PaymentMethod)
                .ToListAsync();

            var report = new PaymentReportDTO
            {
                StartDate = startDate,
                EndDate = endDate,
                Entries = entries,
                GrandTotal = entries.Sum(e => e.TotalAmount)
            };

            return report;
        }
    }
}