using System.ComponentModel.DataAnnotations;

namespace HotelMVCIs.ViewModels
{
    public class RoleModificationVM
    {
        [Required]
        public string RoleName { get; set; }
        public string RoleId { get; set; }
        public string[]? AddIds { get; set; }
        public string[]? DeleteIds { get; set; }
    }
}