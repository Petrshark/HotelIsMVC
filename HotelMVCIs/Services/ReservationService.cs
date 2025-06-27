using HotelMVCIs.Data;
using HotelMVCIs.DTOs;
using HotelMVCIs.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<IEnumerable<ReservationDTO>> GetAllAsync()
        {
            var reservations = await _context.Reservations
                .Include(r => r.Guest)
                .Include(r => r.Room).ThenInclude(room => room.RoomType)
                .Include(r => r.ReservationItems)
                .OrderByDescending(r => r.CheckInDate)
                .ToListAsync();

            var reservationIds = reservations.Select(r => r.Id).ToList();

            var payments = await _context.Payments
                .Where(p => reservationIds.Contains(p.ReservationId))
                .GroupBy(p => p.ReservationId)
                .Select(g => new { ReservationId = g.Key, TotalPaid = g.Sum(p => p.Amount) })
                .ToDictionaryAsync(x => x.ReservationId, x => x.TotalPaid);

            var reservationDTOs = reservations.Select(reservation =>
            {
                var servicesPrice = reservation.ReservationItems.Sum(ri => ri.Quantity * ri.PricePerItem);
                var grandTotal = reservation.TotalPrice + servicesPrice;
                payments.TryGetValue(reservation.Id, out var totalPaid);

                return new ReservationDTO
                {
                    Id = reservation.Id,
                    GuestFullName = $"{reservation.Guest.FirstName} {reservation.Guest.LastName}",
                    RoomNumber = reservation.Room.RoomNumber,
                    RoomTypeName = reservation.Room.RoomType.Name,
                    CheckInDate = reservation.CheckInDate,
                    CheckOutDate = reservation.CheckOutDate,
                    TotalPrice = grandTotal,
                    Status = reservation.Status,
                    RemainingBalance = grandTotal - totalPaid
                };
            }).ToList();

            return reservationDTOs;
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

        public async Task<ReservationDTO?> GetByIdForEditAsync(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null) return null;

            return new ReservationDTO
            {
                Id = reservation.Id,
                GuestId = reservation.GuestId,
                RoomId = reservation.RoomId,
                CheckInDate = reservation.CheckInDate,
                CheckOutDate = reservation.CheckOutDate,
                NumberOfGuests = reservation.NumberOfGuests,
                Status = reservation.Status
            };
        }

        public async Task<Reservation?> GetByIdForDeleteAsync(int id)
        {
            return await _context.Reservations
               .Include(r => r.Guest)
               .Include(r => r.Room).ThenInclude(room => room.RoomType)
               .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<int> CreateAsync(ReservationDTO dto)
        {
            var room = await _context.Rooms.FindAsync(dto.RoomId);
            if (room == null) return 0;

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
            return reservation.Id;
        }

        public async Task UpdateAsync(ReservationDTO dto)
        {
            var reservation = await _context.Reservations.FindAsync(dto.Id);
            var room = await _context.Rooms.FindAsync(dto.RoomId);
            if (reservation == null || room == null) return;

            var nights = (dto.CheckOutDate - dto.CheckInDate).Days;
            if (nights <= 0) nights = 1;

            var roomPrice = nights * room.PricePerNight;

            reservation.GuestId = dto.GuestId;
            reservation.RoomId = dto.RoomId;
            reservation.CheckInDate = dto.CheckInDate;
            reservation.CheckOutDate = dto.CheckOutDate;
            reservation.NumberOfGuests = dto.NumberOfGuests;
            reservation.Status = dto.Status;
            reservation.TotalPrice = roomPrice; // Ukládáme VŽDY jen cenu za ubytování

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

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Reservations.AnyAsync(e => e.Id == id);
        }
    }
}