using System.Collections.Generic;

namespace HotelMVCIs.DTOs
{
    public class RoomRowDTO
    {
        public int RoomId { get; set; }
        public string RoomNumber { get; set; }
        public string RoomTypeName { get; set; }
        public List<ReservationSpanDTO> Reservations { get; set; } = new List<ReservationSpanDTO>();
    }
}