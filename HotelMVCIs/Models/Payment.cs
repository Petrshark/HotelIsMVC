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

        [Required(ErrorMessage = "Částka je povinná.")]
        [Column(TypeName = "decimal(10, 2)")]
        [Range(0.01, 1000000.00, ErrorMessage = "Částka musí být kladné číslo.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Datum platby je povinné.")]
        [DataType(DataType.Date)]
        public DateTime PaymentDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Metoda platby je povinná.")]
        public PaymentMethod PaymentMethod { get; set; } // <--- ZMĚNA: Nyní je to enum

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}