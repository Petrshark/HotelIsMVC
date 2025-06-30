using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelMVCIs.Data;
using HotelMVCIs.DTOs;
using HotelMVCIs.Models;
using HotelMVCIs.Services;
using System.Linq;
using System.Threading.Tasks;

namespace HotelMVCIs.Controllers
{
    [Authorize(Roles = "Admin,Recepční")] // Přístup pouze pro role Admin a Recepční.
    public class GuestsController : Controller
    {
        private readonly GuestService _guestService;
        private readonly HotelMVCIsDbContext _context;

        public GuestsController(GuestService guestService, HotelMVCIsDbContext context)
        {
            _guestService = guestService;
            _context = context;
        }

        // Zobrazí seznam všech hostů.
        public async Task<IActionResult> Index()
        {
            return View(await _guestService.GetAllAsync());
        }

        // Zobrazí formulář pro vytvoření hosta.
        public IActionResult Create()
        {
            return View();
        }

        // Zpracovává vytvoření hosta. Validuje DTO. Chrání před CSRF.
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

        // Zobrazí formulář pro úpravu hosta.
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var dto = await _guestService.GetByIdAsync(id.Value);
            if (dto == null) return NotFound();
            return View(dto);
        }

        // Zpracovává úpravu hosta. Validuje DTO. Chrání před CSRF.
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

        // Zobrazí potvrzení smazání hosta.
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var guest = await _guestService.GetByIdAsync(id.Value);
            if (guest == null) return NotFound();
            return View(guest);
        }

        // Provede smazání hosta. Chrání před CSRF.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _guestService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // Akce pro vytváření hosta přes AJAX (např. z modálního okna).
        // Validuje základní pole a kontroluje duplicitu emailu, vrací JSON výsledek.
        [HttpPost]
        public async Task<IActionResult> CreateFromModal([FromBody] GuestDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.LastName) || string.IsNullOrWhiteSpace(dto.Email))
            {
                return Json(new { success = false, errors = new[] { "Všechna pole jsou povinná." } });
            }

            var existingGuest = await _context.Guests.FirstOrDefaultAsync(g => g.Email == dto.Email);
            if (existingGuest != null)
            {
                return Json(new { success = false, errors = new[] { "Host s tímto emailem již existuje." } });
            }

            if (ModelState.IsValid)
            {
                var guest = new Guest
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email
                };
                _context.Guests.Add(guest);
                await _context.SaveChangesAsync();

                return Json(new { success = true, id = guest.Id, fullName = $"{guest.FirstName} {guest.LastName}" });
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return Json(new { success = false, errors = errors });
        }
    }
}