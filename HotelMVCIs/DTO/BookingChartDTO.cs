using System;
using System.Collections.Generic;

namespace HotelMVCIs.DTOs
{
    public class BookingChartDTO
    {
        public DateTime CurrentMonth { get; set; }
        public List<RoomRowDTO> RoomRows { get; set; } = new List<RoomRowDTO>();
    }
}