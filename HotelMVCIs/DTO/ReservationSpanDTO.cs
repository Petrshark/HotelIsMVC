using HotelMVCIs.Models;
using System;

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

        // ====== NOVÉ VLASTNOSTI PRO DETAILNÍ NÁPOVĚDU ======
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal TotalPaid { get; set; }
    }
}