using System.ComponentModel.DataAnnotations.Schema;

namespace HotelMVCIs.Models
{
    public class ReservationItem
    {
        public int ReservationId { get; set; }
        public virtual Reservation Reservation { get; set; }
        public int HotelServiceId { get; set; }
        public virtual HotelService HotelService { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal PricePerItem { get; set; }
    }
}