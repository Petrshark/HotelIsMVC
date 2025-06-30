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
    public class BookingChartService
    {
        private readonly HotelMVCIsDbContext _context;

        public BookingChartService(HotelMVCIsDbContext context)
        {
            _context = context;
        }

        public async Task<BookingChartDTO> GetBookingChartAsync(DateTime month)
        {
            var startDate = new DateTime(month.Year, month.Month, 1);
            var endDate = startDate.AddMonths(1);

            var rooms = await _context.Rooms
                .Include(r => r.RoomType)
                .OrderBy(r => r.RoomNumber)
                .ToListAsync();

            var reservations = await _context.Reservations
                .Where(r => r.CheckInDate < endDate && r.CheckOutDate > startDate && r.Status != ReservationStatus.Cancelled)
                .Include(r => r.Guest)
                .Include(r => r.Payments)
                .Include(r => r.ReservationItems)
                .OrderBy(r => r.CheckInDate)
                .ToListAsync();

            var chart = new BookingChartDTO
            {
                CurrentMonth = startDate,
                RoomRows = new List<RoomRowDTO>()
            };

            foreach (var room in rooms)
            {
                var roomRow = new RoomRowDTO
                {
                    RoomId = room.Id,
                    RoomNumber = room.RoomNumber,
                    RoomTypeName = room.RoomType.Name
                };

                var reservationsForRoom = reservations.Where(r => r.RoomId == room.Id);

                var lanes = new Dictionary<int, DateTime>();

                foreach (var res in reservationsForRoom)
                {
                    int laneIndex = 0;
                    while (lanes.ContainsKey(laneIndex) && lanes[laneIndex] > res.CheckInDate)
                    {
                        laneIndex++;
                    }
                    lanes[laneIndex] = res.CheckOutDate;

                    var spanStartDate = res.CheckInDate > startDate ? res.CheckInDate : startDate;
                    var spanEndDate = res.CheckOutDate < endDate ? res.CheckOutDate : endDate;

                    var spanDays = (spanEndDate - spanStartDate).Days;

                    if (spanDays > 0)
                    {
                        var servicesPrice = res.ReservationItems.Sum(ri => ri.Quantity * ri.PricePerItem);
                        var grandTotal = res.TotalPrice + servicesPrice;
                        var totalPaid = res.Payments.Sum(p => p.Amount);

                        var span = new ReservationSpanDTO
                        {
                            ReservationId = res.Id,
                            GuestName = $"{res.Guest.FirstName} {res.Guest.LastName}",
                            StartDay = spanStartDate.Day,
                            SpanDays = spanDays,
                            Status = res.Status,
                            LaneIndex = laneIndex,
                            CheckInDate = res.CheckInDate,
                            CheckOutDate = res.CheckOutDate,
                            NumberOfGuests = res.NumberOfGuests,
                            GrandTotal = grandTotal,
                            TotalPaid = totalPaid
                        };
                        roomRow.Reservations.Add(span);
                    }
                }
                chart.RoomRows.Add(roomRow);
            }
            return chart;
        }
    }
}