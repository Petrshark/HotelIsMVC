using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using HotelMVCIs.Models;

namespace HotelMVCIs.DTOs
{
    public class PaymentDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Musíte vybrat rezervaci.")]
        [Display(Name = "Rezervace")]
        public int ReservationId { get; set; }

        // --- NOVINKA: Vlastnosti pro zobrazení detailů rezervace a zbývající částky ---
        // Tyto vlastnosti slouží k naplnění dat pro seznam (Index View) a nejsou určeny pro vstup uživatele
        [Display(Name = "Rezervace")] // Zde můžeme mít obecnější DisplayName
        public string? ReservationDisplay { get; set; } // Např. "#ID - Pokoj Číslo (Jméno Příjmení)"

        [Display(Name = "Celková cena rezervace")]
        public decimal ReservationTotalPrice { get; set; }

        [Display(Name = "Již zaplaceno za rezervaci")]
        public decimal ReservationTotalPaid { get; set; }

        [Display(Name = "Zbývá doplatit za rezervaci")]
        public decimal ReservationRemainingBalance { get; set; }
        // --------------------------------------------------------------------------------

        [Required(ErrorMessage = "Částka je povinná.")]
        [Range(0.01, 1000000.00, ErrorMessage = "Částka musí být kladné číslo a maximálně 1 000 000.")]
        [Display(Name = "Částka")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Datum platby je povinné.")]
        [DataType(DataType.Date)]
        [Display(Name = "Datum platby")]
        public DateTime PaymentDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Metoda platby je povinná.")]
        [Display(Name = "Metoda platby")]
        public PaymentMethod PaymentMethod { get; set; }

        [StringLength(500, ErrorMessage = "Poznámky mohou mít maximálně 500 znaků.")]
        [Display(Name = "Poznámky")]
        public string? Notes { get; set; }

        public IEnumerable<SelectListItem> ReservationsList { get; set; } = new List<SelectListItem>();
    }
}