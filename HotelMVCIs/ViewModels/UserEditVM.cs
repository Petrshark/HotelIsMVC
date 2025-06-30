using System.ComponentModel.DataAnnotations;

namespace HotelMVCIs.ViewModels
{
    public class UserEditVM
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Jméno je povinné.")]
        [Display(Name = "Jméno a příjmení")]
        public string Name { get; set; }

        [Required(ErrorMessage = "E-mail je povinný.")]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Nové heslo")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}