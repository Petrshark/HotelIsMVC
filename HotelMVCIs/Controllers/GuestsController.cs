using Microsoft.AspNetCore.Mvc;
using HotelMVCIs.Services;
using HotelMVCIs.DTOs;
using System.Threading.Tasks;

namespace HotelMVCIs.Controllers
{
    public class GuestsController : Controller
    {
        private readonly GuestService _guestService;

        public GuestsController(GuestService guestService)
        {
            _guestService = guestService;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _guestService.GetAllAsync();
            return View(data);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GuestDTO dto)
        {
            if (ModelState.IsValid)
            {
                await _guestService.CreateAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            return View(dto);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var dto = await _guestService.GetByIdAsync(id.Value);
            if (dto == null) return NotFound();
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, GuestDTO dto)
        {
            if (id != dto.Id) return NotFound();

            if (ModelState.IsValid)
            {
                await _guestService.UpdateAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            return View(dto);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var guest = await _guestService.GetByIdAsync(id.Value);
            if (guest == null) return NotFound();
            return View(guest);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _guestService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}