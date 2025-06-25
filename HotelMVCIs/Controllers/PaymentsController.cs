using Microsoft.AspNetCore.Mvc;
using HotelMVCIs.Services;
using HotelMVCIs.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HotelMVCIs.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly PaymentService _paymentService;
        private readonly ReservationService _reservationService;

        public PaymentsController(PaymentService paymentService, ReservationService reservationService)
        {
            _paymentService = paymentService;
            _reservationService = reservationService;
        }

        // ZMĚNA: Akce Index nyní pracuje s IEnumerable<PaymentDTO>
        public async Task<IActionResult> Index()
        {
            var data = await _paymentService.GetAllAsync(); // Tato metoda nyní vrací IEnumerable<PaymentDTO>
            return View(data);
        }
        public async Task<IActionResult> Create(int? reservationId) // Volitelný parametr pro předvyplnění rezervace
        {
            var dto = new PaymentDTO();
            await PopulateDropdowns(dto);

            if (reservationId.HasValue)
            {
                dto.ReservationId = reservationId.Value;

                // --- ZMĚNA ZDE: Načteme rezervaci, abychom znali její celkovou cenu ---
                var reservation = await _reservationService.GetByIdForDeleteAsync(reservationId.Value);
                if (reservation != null)
                {
                    var totalPaid = await _paymentService.GetTotalPaidForReservationAsync(reservationId.Value);
                    var remainingBalance = reservation.TotalPrice - totalPaid;

                    if (remainingBalance > 0)
                    {
                        dto.Amount = remainingBalance;
                    }
                    else // Pokud už je zaplaceno vše nebo přeplaceno, můžete nastavit na 0 nebo na celkovou cenu jako návrh.
                    {
                        dto.Amount = 0; // Nebo reservation.TotalPrice;
                    }
                }
                // ------------------------------------------------------------------
            }

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PaymentDTO dto)
        {
            // Vlastní validace pro kontrolu, zda platba nepřekračuje celkovou cenu rezervace
            var reservation = await _reservationService.GetByIdForDeleteAsync(dto.ReservationId);
            if (reservation == null)
            {
                ModelState.AddModelError("ReservationId", "Vybraná rezervace neexistuje.");
            }
            else
            {
                var totalPaid = await _paymentService.GetTotalPaidForReservationAsync(dto.ReservationId);
                // Kontrola by měla být proti celkové ceně rezervace, ne proti zbývajícímu dluhu + nové částce
                // Pro Create chceme ověřit, že nová platba + dosud zaplacené nepřekročí TotalPrice
                if (totalPaid + dto.Amount > reservation.TotalPrice + 0.01M) // Malá tolerance pro desetinná čísla
                {
                    ModelState.AddModelError("Amount", $"Zaplacená částka ({totalPaid + dto.Amount:C}) by překročila celkovou cenu rezervace ({reservation.TotalPrice:C}). Zbývá zaplatit {reservation.TotalPrice - totalPaid:C}.");
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

            // Vlastní validace pro kontrolu, zda platba nepřekračuje celkovou cenu rezervace
            var reservation = await _reservationService.GetByIdForDeleteAsync(dto.ReservationId);
            if (reservation == null)
            {
                ModelState.AddModelError("ReservationId", "Vybraná rezervace neexistuje.");
            }
            else
            {
                // Musíme odečíst stávající částku této platby před kontrolou
                var currentPayment = await _paymentService.GetByIdAsync(id);
                decimal amountBeforeEdit = currentPayment?.Amount ?? 0;

                var totalPaidExcludingCurrent = await _paymentService.GetTotalPaidForReservationAsync(dto.ReservationId) - amountBeforeEdit;

                if (totalPaidExcludingCurrent + dto.Amount > reservation.TotalPrice + 0.01M)
                {
                    ModelState.AddModelError("Amount", $"Upravená částka ({totalPaidExcludingCurrent + dto.Amount:C}) by překročila celkovou cenu rezervace ({reservation.TotalPrice:C}). Zbývá zaplatit {reservation.TotalPrice - totalPaidExcludingCurrent:C}.");
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
            var payment = await _paymentService.GetByIdAsync(id.Value); // Changed to use GetByIdAsync

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

        // Pomocná metoda pro naplnění rozevíracích seznamů
        private async Task PopulateDropdowns(PaymentDTO dto)
        {
            dto.ReservationsList = await _paymentService.GetReservationsForDropdownAsync();
        }
    }
}