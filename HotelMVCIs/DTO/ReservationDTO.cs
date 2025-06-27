using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using HotelMVCIs.Models;
using System.Collections.Generic;
using System;

namespace HotelMVCIs.DTOs
{
    public class ReservationDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Musíte vybrat hosta.")]
        [Display(Name = "Host")]
        public int GuestId { get; set; }

        public string? GuestFullName { get; set; }

        [Required(ErrorMessage = "Musíte vybrat pokoj.")]
        [Display(Name = "Pokoj")]
        public int RoomId { get; set; }

        public string? RoomNumber { get; set; }
        public string? RoomTypeName { get; set; }

        [Required(ErrorMessage = "Datum příjezdu je povinné.")]
        [DataType(DataType.Date)]
        [Display(Name = "Datum příjezdu (Check-in)")]
        public DateTime CheckInDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Datum odjezdu je povinné.")]
        [DataType(DataType.Date)]
        [Display(Name = "Datum odjezdu (Check-out)")]
        public DateTime CheckOutDate { get; set; } = DateTime.Today.AddDays(1);

        [Required(ErrorMessage = "Počet hostů je povinný.")]
        [Range(1, 10)]
        [Display(Name = "Počet hostů")]
        public int NumberOfGuests { get; set; }

        [Display(Name = "Celková cena")]
        public decimal TotalPrice { get; set; }

        [Required(ErrorMessage = "Status je povinný.")]
        [Display(Name = "Status")]
        public ReservationStatus Status { get; set; }

        [Display(Name = "Zbývá doplatit")]
        public decimal RemainingBalance { get; set; }

        public IEnumerable<SelectListItem> GuestsList { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> RoomsList { get; set; } = new List<SelectListItem>();
    }
}