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

        public async Task<BookingChartDTO> GetChartDataAsync(int year, int month)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1);

            var rooms = await _context.Rooms
                .Include(r => r.RoomType)
                .OrderBy(r => r.RoomNumber)
                .ToListAsync();

            var reservations = await _context.Reservations
                .Include(r => r.Guest)
                .Where(r => r.CheckInDate < endDate && r.CheckOutDate > startDate && r.Status != ReservationStatus.Cancelled)
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

                var lanes = new List<List<ReservationSpanDTO>>();
                var reservationsForRoom = reservations
                    .Where(r => r.RoomId == room.Id)
                    .OrderBy(r => r.CheckInDate);

                foreach (var res in reservationsForRoom)
                {
                    var resStartDate = res.CheckInDate < startDate ? startDate : res.CheckInDate;
                    var resEndDate = res.CheckOutDate > endDate ? endDate : res.CheckOutDate;

                    var span = new ReservationSpanDTO
                    {
                        ReservationId = res.Id,
                        GuestName = $"{res.Guest.FirstName} {res.Guest.LastName}",
                        StartDay = resStartDate.Day,
                        SpanDays = (resEndDate - resStartDate).Days == 0 ? 1 : (resEndDate - resStartDate).Days,
                        Status = res.Status
                    };

                    int assignedLane = -1;
                    for (int i = 0; i < lanes.Count; i++)
                    {
                        bool overlaps = lanes[i].Any(existingSpan =>
                        {
                            int newStart = span.StartDay;
                            int newEnd = span.StartDay + span.SpanDays;
                            int existingStart = existingSpan.StartDay;
                            int existingEnd = existingSpan.StartDay + existingSpan.SpanDays;
                            return newStart < existingEnd && newEnd > existingStart;
                        });

                        if (!overlaps)
                        {
                            assignedLane = i;
                            break;
                        }
                    }

                    if (assignedLane == -1)
                    {
                        assignedLane = lanes.Count;
                        lanes.Add(new List<ReservationSpanDTO>());
                    }

                    span.LaneIndex = assignedLane;
                    lanes[assignedLane].Add(span);
                    roomRow.Reservations.Add(span);
                }
                chart.RoomRows.Add(roomRow);
            }

            return chart;
        }
    }
}