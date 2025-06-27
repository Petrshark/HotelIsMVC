using Microsoft.AspNetCore.Mvc;
using HotelMVCIs.Services;
using HotelMVCIs.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using HotelMVCIs.Data; // <-- Důležitý using pro DbContext

namespace HotelMVCIs.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly PaymentService _paymentService;
        private readonly ReservationService _reservationService;
        private readonly HotelMVCIsDbContext _context; // <-- PŘIDANÁ PROMĚNNÁ PRO DB KONTEXT

        public PaymentsController(
            PaymentService paymentService,
            ReservationService reservationService,
            HotelMVCIsDbContext context) // <-- PŘIDANÁ ZÁVISLOST V KONSTRUKTORU
        {
            _paymentService = paymentService;
            _reservationService = reservationService;
            _context = context; // <-- PŘIŘAZENÍ KONTEXTU
        }

        public async Task<IActionResult> Index()
        {
            var data = await _paymentService.GetAllAsync();
            return View(data);
        }

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
                var servicesPrice = await _context.ReservationItems
                    .Where(ri => ri.ReservationId == dto.ReservationId)
                    .SumAsync(ri => ri.Quantity * ri.PricePerItem);
                var grandTotal = reservation.TotalPrice + servicesPrice;
                var totalPaid = await _paymentService.GetTotalPaidForReservationAsync(dto.ReservationId);

                if (totalPaid + dto.Amount > grandTotal + 0.01M)
                {
                    ModelState.AddModelError("Amount", $"Zaplacená částka ({totalPaid + dto.Amount:C}) by překročila celkovou cenu rezervace ({grandTotal:C}).");
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

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var dto = await _paymentService.GetByIdAsync(id.Value);
            if (dto == null) return NotFound();

            await PopulateDropdowns(dto);
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PaymentDTO dto)
        {
            if (id != dto.Id) return NotFound();

            var reservation = await _reservationService.GetByIdForDeleteAsync(dto.ReservationId);
            if (reservation == null)
            {
                ModelState.AddModelError("ReservationId", "Vybraná rezervace neexistuje.");
            }
            else
            {
                var currentPayment = await _paymentService.GetByIdAsync(id);
                decimal amountBeforeEdit = currentPayment?.Amount ?? 0;
                var totalPaidExcludingCurrent = await _paymentService.GetTotalPaidForReservationAsync(dto.ReservationId) - amountBeforeEdit;
                var servicesPrice = await _context.ReservationItems.Where(ri => ri.ReservationId == dto.ReservationId).SumAsync(ri => ri.Quantity * ri.PricePerItem);
                var grandTotal = reservation.TotalPrice + servicesPrice;

                if (totalPaidExcludingCurrent + dto.Amount > grandTotal + 0.01M)
                {
                    ModelState.AddModelError("Amount", $"Upravená částka ({totalPaidExcludingCurrent + dto.Amount:C}) by překročila celkovou cenu rezervace ({grandTotal:C}).");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _paymentService.UpdateAsync(dto);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _paymentService.ExistsAsync(dto.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            await PopulateDropdowns(dto);
            return View(dto);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var payment = await _paymentService.GetPaymentForDeleteAsync(id.Value);
            if (payment == null) return NotFound();
            return View(payment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _paymentService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // =================================================================
        // TATO METODA CHYBĚLA
        // =================================================================
        private async Task PopulateDropdowns(PaymentDTO dto)
        {
            dto.ReservationsList = await _paymentService.GetReservationsForDropdownAsync();
        }
    }
}