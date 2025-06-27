using HotelMVCIs.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace HotelMVCIs.DTOs
{
    public class ReportEntryDTO
    {
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Display(Name = "Metoda platby")]
        public PaymentMethod PaymentMethod { get; set; }

        [Display(Name = "Celková částka")]
        public decimal TotalAmount { get; set; }
    }
}