using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using HotelMVCIs.DTOs;
using HotelMVCIs.Services;
using System.Threading.Tasks;

namespace HotelMVCIs.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RoomsController : Controller
    {
        private readonly RoomService _roomService;
        private readonly RoomTypeService _roomTypeService;

        public RoomsController(RoomService roomService, RoomTypeService roomTypeService)
        {
            _roomService = roomService;
            _roomTypeService = roomTypeService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _roomService.GetAllAsync());
        }

        public async Task<IActionResult> Create()
        {
            var model = new RoomDTO
            {
                RoomTypesList = new SelectList(await _roomTypeService.GetAllAsync(), "Id", "Name")
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoomDTO dto)
        {
            if (ModelState.IsValid)
            {
                await _roomService.CreateAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            dto.RoomTypesList = new SelectList(await _roomTypeService.GetAllAsync(), "Id", "Name", dto.RoomTypeId);
            return View(dto);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var dto = await _roomService.GetByIdAsync(id.Value);
            if (dto == null) return NotFound();
            dto.RoomTypesList = new SelectList(await _roomTypeService.GetAllAsync(), "Id", "Name", dto.RoomTypeId);
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RoomDTO dto)
        {
            if (id != dto.Id) return NotFound();
            if (ModelState.IsValid)
            {
                await _roomService.UpdateAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            dto.RoomTypesList = new SelectList(await _roomTypeService.GetAllAsync(), "Id", "Name", dto.RoomTypeId);
            return View(dto);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var room = await _roomService.GetRoomForDeleteAsync(id.Value);
            if (room == null) return NotFound();
            return View(room);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _roomService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}