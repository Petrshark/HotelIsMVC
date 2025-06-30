using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelMVCIs.Data;
using HotelMVCIs.DTOs;
using HotelMVCIs.Models;
using HotelMVCIs.Services;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace HotelMVCIs.Controllers
{
    [Authorize(Roles = "Admin,Recepční")] // Přístup pouze pro role Admin a Recepční.
    public class PaymentsController : Controller
    {
        private readonly PaymentService _paymentService;
        private readonly ReservationService _reservationService;
        private readonly HotelMVCIsDbContext _context;

        public PaymentsController(PaymentService paymentService, ReservationService reservationService, HotelMVCIsDbContext context)
        {
            _paymentService = paymentService;
            _reservationService = reservationService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _paymentService.GetAllAsync());
        }

        // Zobrazí formulář pro vytvoření platby. Může předvyplnit částku a rezervaci.
        public async Task<IActionResult> Create(int? reservationId)
        {
            var dto = new PaymentDTO();
            await PopulateDropdowns(dto);

            if (reservationId.HasValue)
            {
                dto.ReservationId = reservationId.Value;
                var reservation = await _reservationService.GetByIdForDeleteAsync(reservationId.Value);
                if (reservation != null)
                {
                    // Vypočítá celkovou cenu rezervace (ubytování + služby) a zbývající dlužnou částku.
                    var servicesPrice = await _context.ReservationItems
                        .Where(ri => ri.ReservationId == reservationId.Value)
                        .SumAsync(ri => ri.Quantity * ri.PricePerItem);
                    var grandTotal = reservation.TotalPrice + servicesPrice;
                    var totalPaid = await _paymentService.GetTotalPaidForReservationAsync(reservationId.Value);
                    var remainingBalance = grandTotal - totalPaid;
                    dto.Amount = remainingBalance > 0 ? remainingBalance : 0;
                }
            }
            return View(dto);
        }

        // Zpracovává vytvoření platby. Validuje DTO a kontroluje nepřekročení dlužné částky. Chrání před CSRF.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PaymentDTO dto)
        {
            var reservation = await _reservationService.GetByIdForDeleteAsync(dto.ReservationId);
            if (reservation == null)
            {
                ModelState.AddModelError("ReservationId", "Vybraná rezervace neexistuje.");
            }
            else
            {
                var servicesPrice = await _context.ReservationItems.Where(ri => ri.ReservationId == dto.ReservationId).SumAsync(ri => ri.Quantity * ri.PricePerItem);
                var grandTotal = reservation.TotalPrice + servicesPrice;
                var totalPaid = await _paymentService.GetTotalPaidForReservationAsync(dto.ReservationId);

                // Brání přeplacení rezervace.
                if (totalPaid + dto.Amount > grandTotal + 0.01M)
                {
                    ModelState.AddModelError("Amount", $"Zaplacená částka ({(totalPaid + dto.Amount):C}) by překročila celkovou cenu rezervace ({grandTotal:C}).");
                }
            }

            if (ModelState.IsValid)
            {
                await _paymentService.CreateAsync(dto);
                return RedirectToAction(nameof(Index));
            }

            await PopulateDropdowns(dto);
            return View(dto);
        }

        // Zobrazí formulář pro úpravu platby.
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var dto = await _paymentService.GetByIdAsync(id.Value);
            if (dto == null) return NotFound();

            await PopulateDropdowns(dto);
            return View(dto);
        }

        // Zpracovává úpravu platby. Validuje DTO. Chrání před CSRF.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PaymentDTO dto)
        {
            if (id != dto.Id) return NotFound();

            if (ModelState.IsValid)
            {
                await _paymentService.UpdateAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            await PopulateDropdowns(dto);
            return View(dto);
        }

        // Zobrazí potvrzení smazání platby.
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var payment = await _paymentService.GetPaymentForDeleteAsync(id.Value);
            if (payment == null) return NotFound();
            return View(payment);
        }

        // Provede smazání platby. Chrání před CSRF.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _paymentService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // Pomocná metoda pro naplnění rozbalovacích seznamů rezervací.
        private async Task PopulateDropdowns(PaymentDTO dto)
        {
            dto.ReservationsList = await _paymentService.GetReservationsForDropdownAsync();
        }
    }
}