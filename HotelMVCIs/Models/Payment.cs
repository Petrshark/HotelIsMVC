using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelMVCIs.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int ReservationId { get; set; }
        [ForeignKey("ReservationId")]
        public virtual Reservation Reservation { get; set; }
        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        [Range(0.01, 1000000.00)]
        public decimal Amount { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime PaymentDate { get; set; } = DateTime.Now;
        [Required]
        public PaymentMethod PaymentMethod { get; set; }
        [StringLength(500)]
        public string? Notes { get; set; }
    }
}