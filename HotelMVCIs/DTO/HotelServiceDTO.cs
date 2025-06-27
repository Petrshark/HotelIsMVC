using System.ComponentModel.DataAnnotations;

namespace HotelMVCIs.DTOs
{
    public class HotelServiceDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Název služby je povinný.")]
        [StringLength(100)]
        [Display(Name = "Název služby")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Cena je povinná.")]
        [Range(0.01, 100000.00)]
        [Display(Name = "Cena")]
        public decimal Price { get; set; }

        [StringLength(500)]
        [Display(Name = "Popis")]
        public string? Description { get; set; }

        [Display(Name = "Aktivní služba")]
        public bool IsActive { get; set; }
    }
}