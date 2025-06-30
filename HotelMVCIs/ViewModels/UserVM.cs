using System.ComponentModel.DataAnnotations;

namespace HotelMVCIs.ViewModels
{
    public class UserVM
    {
        [Required(ErrorMessage = "Jméno je povinné.")]
        [Display(Name = "Jméno a příjmení")]
        public string Name { get; set; }

        [Required(ErrorMessage = "E-mail je povinný.")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail není ve správném formátu.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Heslo je povinné.")]
        [DataType(DataType.Password)]
        [Display(Name = "Heslo")]
        public string Password { get; set; }
    }
}