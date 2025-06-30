using Microsoft.AspNetCore.Identity;
using System.Collections.Generic; // Tento using musí být zde, nahoře

namespace HotelMVCIs.Models
{
    public class AppUser : IdentityUser
    {
        public string? Name { get; set; }

        // Vlastnost pro rezervace tady být nemusí, je na druhé straně vazby
        // public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}