using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace HotelMVCIs.Models
{
    public class Guest
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(50)]
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }
        [StringLength(20)]
        public string? PhoneNumber { get; set; }
        [StringLength(100)]
        public string? Address { get; set; }
        [StringLength(50)]
        public string? City { get; set; }
        [StringLength(20)]
        public string? PostalCode { get; set; }
        [StringLength(50)]
        public string? Nationality { get; set; }
        public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}