using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using HotelMVCIs.Models;

namespace HotelMVCIs.ViewModels
{
    public class RoleEditVM
    {
        public IdentityRole Role { get; set; }
        public IEnumerable<AppUser> Members { get; set; }
        public IEnumerable<AppUser> NonMembers { get; set; }
    }
}