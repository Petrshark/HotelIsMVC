using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelMVCIs.Models
{
    public class HotelService
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Název služby je povinný.")]
        [StringLength(100, ErrorMessage = "Název služby může mít maximálně 100 znaků.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Cena je povinná.")]
        [Column(TypeName = "decimal(10, 2)")]
        [Range(0.01, 100000.00, ErrorMessage = "Cena musí být platné číslo a větší než 0.")]
        public decimal Price { get; set; }

        [StringLength(500)]
        public string? Description { get; set; } // Volitelný popis služby

        [Display(Name = "Aktivní")]
        public bool IsActive { get; set; } = true; // Určuje, zda je služba aktuálně aktivní/nabízená
    }
}