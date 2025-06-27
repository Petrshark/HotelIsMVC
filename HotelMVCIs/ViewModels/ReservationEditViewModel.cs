using HotelMVCIs.DTOs;
using HotelMVCIs.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HotelMVCIs.ViewModels
{
    public class ReservationEditViewModel
    {
        public ReservationDTO Reservation { get; set; }
        public List<ReservationItem> AddedServices { get; set; } = new List<ReservationItem>();
        public IEnumerable<SelectListItem> AvailableServices { get; set; } = new List<SelectListItem>();
        public int ServiceToAddId { get; set; }
        public int ServiceToAddQuantity { get; set; } = 1;

        [Display(Name = "Cena za ubytování")]
        public decimal AccommodationPrice { get; set; }
        [Display(Name = "Cena za služby")]
        public decimal ServicesPrice { get; set; }
        [Display(Name = "Celková částka")]
        public decimal GrandTotal { get; set; }
        [Display(Name = "Zaplaceno")]
        public decimal TotalPaid { get; set; }
        [Display(Name = "Zbývá doplatit")]
        public decimal RemainingBalance { get; set; }
    }
}