﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using HotelMVCIs.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelMVCIs.DTOs
{
    public class PaymentDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Musíte vybrat rezervaci.")]
        [Display(Name = "Rezervace")]
        public int ReservationId { get; set; }

        [Display(Name = "Rezervace")]
        public string? ReservationDisplay { get; set; }

        [Display(Name = "Celková cena rezervace")]
        public decimal ReservationTotalPrice { get; set; }

        [Display(Name = "Již zaplaceno")]
        public decimal ReservationTotalPaid { get; set; }

        [Display(Name = "Zbývá doplatit")]
        public decimal ReservationRemainingBalance { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
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

        [StringLength(500)]
        [Display(Name = "Poznámky")]
        public string? Notes { get; set; }

        public IEnumerable<SelectListItem> ReservationsList { get; set; } = new List<SelectListItem>();
    }
}