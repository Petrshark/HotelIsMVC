using System.Collections.Generic;
using System;

namespace HotelMVCIs.DTOs
{
    public class BookingChartDTO
    {
        public DateTime CurrentMonth { get; set; }
        public List<RoomRowDTO> RoomRows { get; set; } = new List<RoomRowDTO>();
    }
}