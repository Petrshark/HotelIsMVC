using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using HotelMVCIs.Models;

namespace HotelMVCIs.DTOs
{
    public class ReservationDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Musíte vybrat hosta.")]
        [Display(Name = "Host")]
        public int GuestId { get; set; }
        // Tyto vlastnosti jsou POUZE PRO ZOBRAZENÍ v seznamu,
        // nemají být Required ve formuláři Edit/Create
        // A pokud nemají být editable, neměly by mít ani [Display(Name=...)]
        // proto jsem je odstranil.
        public string? GuestFullName { get; set; } // <--- ZMĚNA: ? pro nullable string
        public string? RoomNumber { get; set; } // <--- ZMĚNA: ? pro nullable string
        public string? RoomTypeName { get; set; } // <--- ZMĚNA: ? pro nullable string


        [Required(ErrorMessage = "Musíte vybrat pokoj.")]
        [Display(Name = "Pokoj")]
        public int RoomId { get; set; }

        [Required(ErrorMessage = "Datum příjezdu je povinné.")]
        [DataType(DataType.Date)]
        [Display(Name = "Datum příjezdu (Check-in)")]
        public DateTime CheckInDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Datum odjezdu je povinné.")]
        [DataType(DataType.Date)]
        [Display(Name = "Datum odjezdu (Check-out)")]
        public DateTime CheckOutDate { get; set; } = DateTime.Today.AddDays(1);

        [Required(ErrorMessage = "Počet hostů je povinný.")]
        [Range(1, 10, ErrorMessage = "Počet hostů musí být alespoň 1.")]
        [Display(Name = "Počet hostů")]
        public int NumberOfGuests { get; set; }

        // TotalPrice je vypočítaná, neměla by být Required ani Display pro formulář
        public decimal TotalPrice { get; set; }

        [Required(ErrorMessage = "Status je povinný.")]
        [Display(Name = "Status")]
        public ReservationStatus Status { get; set; }

        // RemainingBalance je také vypočítaná pro zobrazení
        [Display(Name = "Zbývá doplatit")]
        public decimal RemainingBalance { get; set; }

        public IEnumerable<SelectListItem> GuestsList { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> RoomsList { get; set; } = new List<SelectListItem>();
    }
}