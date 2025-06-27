using Microsoft.AspNetCore.Mvc;
using HotelMVCIs.Services;
using HotelMVCIs.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HotelMVCIs.Controllers
{
    public class RoomTypesController : Controller
    {
        private readonly RoomTypeService _service;

        public RoomTypesController(RoomTypeService service)
        {
            _service = service;
        }

        // GET: RoomTypes
        public async Task<IActionResult> Index()
        {
            var data = await _service.GetAllAsync();
            return View(data);
        }

        // GET: RoomTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: RoomTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoomTypeDTO dto)
        {
            if (ModelState.IsValid)
            {
                await _service.CreateAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            return View(dto);
        }

        // GET: RoomTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var dto = await _service.GetByIdAsync(id.Value);
            if (dto == null) return NotFound();
            return View(dto);
        }

        // POST: RoomTypes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RoomTypeDTO dto)
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

        // GET: RoomTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var dto = await _service.GetByIdAsync(id.Value);
            if (dto == null) return NotFound();
            return View(dto);
        }

        // POST: RoomTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}