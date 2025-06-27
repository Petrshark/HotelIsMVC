using Microsoft.AspNetCore.Mvc;
using HotelMVCIs.Services;
using HotelMVCIs.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HotelMVCIs.Controllers
{
    public class RoomsController : Controller
    {
        private readonly RoomService _roomService;
        private readonly RoomTypeService _roomTypeService;

        public RoomsController(RoomService roomService, RoomTypeService roomTypeService)
        {
            _roomService = roomService;
            _roomTypeService = roomTypeService;
        }

        // GET: Rooms
        public async Task<IActionResult> Index()
        {
            var data = await _roomService.GetAllAsync();
            return View(data);
        }

        // GET: Rooms/Create
        public async Task<IActionResult> Create()
        {
            var types = await _roomTypeService.GetAllAsync();
            var model = new RoomDTO
            {
                RoomTypesList = new SelectList(types, "Id", "Name")
            };
            return View(model);
        }

        // POST: Rooms/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoomDTO dto)
        {
            if (ModelState.IsValid)
            {
                await _roomService.CreateAsync(dto);
                return RedirectToAction(nameof(Index));
            }

            var types = await _roomTypeService.GetAllAsync();
            dto.RoomTypesList = new SelectList(types, "Id", "Name", dto.RoomTypeId);
            return View(dto);
        }

        // GET: Rooms/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var dto = await _roomService.GetByIdAsync(id.Value);
            if (dto == null) return NotFound();

            var types = await _roomTypeService.GetAllAsync();
            dto.RoomTypesList = new SelectList(types, "Id", "Name", dto.RoomTypeId);
            return View(dto);
        }

        // POST: Rooms/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RoomDTO dto)
        {
            if (id != dto.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _roomService.UpdateAsync(dto);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _roomService.ExistsAsync(dto.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            var types = await _roomTypeService.GetAllAsync();
            dto.RoomTypesList = new SelectList(types, "Id", "Name", dto.RoomTypeId);
            return View(dto);
        }

        // GET: Rooms/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var roomToDelete = await _roomService.GetRoomForDeleteAsync(id.Value);

            if (roomToDelete == null) return NotFound();
            return View(roomToDelete);
        }

        // POST: Rooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _roomService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}