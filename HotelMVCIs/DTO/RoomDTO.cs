using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelMVCIs.DTOs
{
    public class RoomDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Číslo pokoje je povinné.")]
        [StringLength(10)]
        [Display(Name = "Číslo pokoje")]
        public string RoomNumber { get; set; }

        [Required(ErrorMessage = "Musíte vybrat typ pokoje.")]
        [Display(Name = "Typ pokoje")]
        public int RoomTypeId { get; set; }

        [Required(ErrorMessage = "Cena je povinná.")]
        [Range(0.01, 100000.00, ErrorMessage = "Cena musí být platné číslo.")]
        [Display(Name = "Cena za noc")]
        public decimal PricePerNight { get; set; }

        [StringLength(500)]
        [Display(Name = "Popis")]
        public string Description { get; set; }

        public IEnumerable<SelectListItem> RoomTypesList { get; set; } = new List<SelectListItem>();
    }
}