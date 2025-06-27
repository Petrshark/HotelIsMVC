using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelMVCIs.Models
{
    public class Reservation
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int GuestId { get; set; }
        [ForeignKey("GuestId")]
        public virtual Guest Guest { get; set; }
        [Required]
        public int RoomId { get; set; }
        [ForeignKey("RoomId")]
        public virtual Room Room { get; set; }
        [Required]
        public DateTime CheckInDate { get; set; }
        [Required]
        public DateTime CheckOutDate { get; set; }
        [Required]
        public int NumberOfGuests { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal TotalPrice { get; set; } // Cena POUZE za ubytování
        public ReservationStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public virtual ICollection<ReservationItem> ReservationItems { get; set; } = new List<ReservationItem>();
    }
}