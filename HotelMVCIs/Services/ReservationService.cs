using HotelMVCIs.Data;
using HotelMVCIs.DTOs;
using HotelMVCIs.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public ReservationService(HotelMVCIsDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ReservationDTO>> GetAllAsync()
        {
            var reservations = await _context.Reservations
                .Include(r => r.Guest)
                .Include(r => r.Room).ThenInclude(room => room.RoomType)
                .Include(r => r.ReservationItems)
                .Include(r => r.Payments)
                .OrderByDescending(r => r.CheckInDate)
                .ToListAsync();

            return reservations.Select(r =>
            {
                var servicesPrice = r.ReservationItems.Sum(ri => ri.Quantity * ri.PricePerItem);
                var grandTotal = r.TotalPrice + servicesPrice;
                var totalPaid = r.Payments.Sum(p => p.Amount);

                return new ReservationDTO
                {
                    Id = r.Id,
                    GuestFullName = $"{r.Guest.FirstName} {r.Guest.LastName}",
                    RoomNumber = r.Room.RoomNumber,
                    RoomTypeName = r.Room.RoomType.Name,
                    CheckInDate = r.CheckInDate,
                    CheckOutDate = r.CheckOutDate,
                    TotalPrice = grandTotal,
                    RemainingBalance = grandTotal - totalPaid,
                    Status = r.Status
                };
            }).ToList();
        }

        public async Task<ReservationDTO?> GetByIdAsync(int id)
        {
            var reservation = await _context.Reservations
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);

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

            var reservation = new Reservation
            {
                GuestId = dto.GuestId,
                RoomId = dto.RoomId,
                CheckInDate = dto.CheckInDate,
                CheckOutDate = dto.CheckOutDate,
                NumberOfGuests = dto.NumberOfGuests,
                Status = dto.Status,
                TotalPrice = room.PricePerNight * nights
            };
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();
            return reservation.Id;
        }

        public async Task UpdateAsync(ReservationDTO dto)
        {
            var reservation = await _context.Reservations.FindAsync(dto.Id);
            if (reservation != null)
            {
                var room = await _context.Rooms.FindAsync(dto.RoomId);
                if (room == null) return;
                var numberOfNights = (dto.CheckOutDate - dto.CheckInDate).Days;
                if (numberOfNights <= 0) numberOfNights = 1;

                reservation.GuestId = dto.GuestId;
                reservation.RoomId = dto.RoomId;
                reservation.CheckInDate = dto.CheckInDate;
                reservation.CheckOutDate = dto.CheckOutDate;
                reservation.NumberOfGuests = dto.NumberOfGuests;
                reservation.Status = dto.Status;
                reservation.TotalPrice = room.PricePerNight * numberOfNights;

                _context.Update(reservation);
                await _context.SaveChangesAsync();
            }
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
    }
}