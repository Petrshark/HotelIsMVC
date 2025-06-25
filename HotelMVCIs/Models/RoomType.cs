using System.ComponentModel.DataAnnotations;

namespace HotelMVCIs.Models
{
    public class RoomType
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Název je povinný.")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Kapacita je povinná.")]
        [Range(1, 10, ErrorMessage = "Kapacita musí být od 1 do 10.")]
        public int Capacity { get; set; }

        public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}