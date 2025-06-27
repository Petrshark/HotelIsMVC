using Microsoft.AspNetCore.Mvc;
using HotelMVCIs.Services;
using HotelMVCIs.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HotelMVCIs.Controllers
{
    public class HotelServicesController : Controller
    {
        private readonly HotelServiceService _service;

        public HotelServicesController(HotelServiceService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _service.GetAllAsync();
            return View(data);
        }

        public IActionResult Create()
        {
            return View();
        }

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

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var dto = await _service.GetByIdAsync(id.Value);
            if (dto == null) return NotFound();
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HotelServiceDTO dto)
        {
            if (id != dto.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _service.UpdateAsync(dto);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _service.ExistsAsync(dto.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(dto);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var dto = await _service.GetByIdAsync(id.Value);
            if (dto == null) return NotFound();
            return View(dto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}