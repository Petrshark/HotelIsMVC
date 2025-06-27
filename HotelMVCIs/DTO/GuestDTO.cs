using System;
using System.ComponentModel.DataAnnotations;

namespace HotelMVCIs.DTOs
{
    public class GuestDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Křestní jméno je povinné.")]
        [StringLength(50)]
        [Display(Name = "Křestní jméno")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Příjmení je povinné.")]
        [StringLength(50)]
        [Display(Name = "Příjmení")]
        public string LastName { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Datum narození")]
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Email je povinný.")]
        [EmailAddress(ErrorMessage = "Zadejte platnou emailovou adresu.")]
        [StringLength(100)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [StringLength(20)]
        [Phone(ErrorMessage = "Zadejte platné telefonní číslo.")]
        [Display(Name = "Telefonní číslo")]
        public string? PhoneNumber { get; set; }

        [StringLength(100)]
        [Display(Name = "Adresa (ulice a č.p.)")]
        public string? Address { get; set; }

        [StringLength(50)]
        [Display(Name = "Město")]
        public string? City { get; set; }

        [StringLength(20)]
        [Display(Name = "PSČ")]
        public string? PostalCode { get; set; }

        [StringLength(50)]
        [Display(Name = "Národnost")]
        public string? Nationality { get; set; }
    }
}