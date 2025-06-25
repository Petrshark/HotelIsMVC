using HotelMVCIs.Models;

namespace HotelMVCIs.DTOs
{
    public class ReservationSpanDTO
    {
        public int ReservationId { get; set; }
        public string GuestName { get; set; }
        public int StartDay { get; set; }
        public int SpanDays { get; set; }
        public ReservationStatus Status { get; set; }
        public int LaneIndex { get; set; }
    }
}