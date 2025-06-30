using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelMVCIs.Models
{
    public class HotelService
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        [Range(0.01, 100000.00)]
        public decimal Price { get; set; }
        [StringLength(500)]
        public string? Description { get; set; }
        [Display(Name = "Aktivní")]
        public bool IsActive { get; set; } = true;
        public virtual ICollection<ReservationItem> ReservationItems { get; set; } = new List<ReservationItem>();
    }
}