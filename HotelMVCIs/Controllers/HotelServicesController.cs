using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HotelMVCIs.DTOs;
using HotelMVCIs.Services;
using System.Threading.Tasks;

namespace HotelMVCIs.Controllers
{
    [Authorize(Roles = "Admin")] // Přístup pouze pro roli Admin.
    public class HotelServicesController : Controller
    {
        private readonly HotelServiceService _service;

        public HotelServicesController(HotelServiceService service)
        {
            _service = service;
        }

        // Zobrazí seznam všech hotelových služeb.
        public async Task<IActionResult> Index()
        {
            return View(await _service.GetAllAsync());
        }

        // Zobrazí formulář pro vytvoření služby.
        public IActionResult Create()
        {
            return View();
        }

        // Zpracovává vytvoření služby. Validuje DTO. Chrání před CSRF.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HotelServiceDTO dto)
        {
            if (ModelState.IsValid)
            {
                await _service.CreateAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            return View(dto);
        }

        // Zobrazí formulář pro úpravu služby.
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var dto = await _service.GetByIdAsync(id.Value);
            if (dto == null) return NotFound();
            return View(dto);
        }

        // Zpracovává úpravu služby. Validuje DTO. Chrání před CSRF.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HotelServiceDTO dto)
        {
            if (id != dto.Id) return NotFound();

            if (ModelState.IsValid)
            {
                await _service.UpdateAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            return View(dto);
        }

        // Zobrazí potvrzení smazání služby.
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var dto = await _service.GetByIdAsync(id.Value);
            if (dto == null) return NotFound();
            return View(dto);
        }

        // Provede smazání služby. Chrání před CSRF.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}