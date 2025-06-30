using System.ComponentModel.DataAnnotations;

namespace HotelMVCIs.ViewModels
{
    public class LoginVM
    {
        [Required(ErrorMessage = "E-mail je povinný.")]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Heslo je povinné.")]
        [DataType(DataType.Password)]
        [Display(Name = "Heslo")]
        public string Password { get; set; }

        [Display(Name = "Pamatovat si mě?")]
        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
    }
}