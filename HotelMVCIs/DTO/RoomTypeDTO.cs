using System.ComponentModel.DataAnnotations;

namespace HotelMVCIs.DTOs
{
    public class RoomTypeDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Název je povinný.")]
        [StringLength(100)]
        [Display(Name = "Název typu pokoje")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Kapacita je povinná.")]
        [Range(1, 10)]
        [Display(Name = "Maximální kapacita (osob)")]
        public int Capacity { get; set; }
    }
}