using Microsoft.AspNetCore.Mvc;
using HotelMVCIs.Services;
using HotelMVCIs.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelMVCIs.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly ReservationService _reservationService;
        private readonly GuestService _guestService;
        private readonly RoomService _roomService;

        public ReservationsController(ReservationService reservationService, GuestService guestService, RoomService roomService)
        {
            _reservationService = reservationService;
            _guestService = guestService;
            _roomService = roomService;
        }

        // ZMĚNA: Akce Index nyní pracuje s ReservationDTO, které je obohaceno
        public async Task<IActionResult> Index()
        {
            var reservations = await _reservationService.GetAllAsync(); // Tato metoda nyní vrací IEnumerable<ReservationDTO>
            return View(reservations);
        }
        public async Task<IActionResult> Create()
        {
            var dto = new ReservationDTO();
            await PopulateDropdowns(dto);
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReservationDTO dto)
        {
            if (dto.CheckOutDate <= dto.CheckInDate)
            {
                ModelState.AddModelError("CheckOutDate", "Datum odjezdu musí být po datu příjezdu.");
            }

            var selectedRoom = await _roomService.GetRoomForDeleteAsync(dto.RoomId);
            if (selectedRoom != null && dto.NumberOfGuests > selectedRoom.RoomType.Capacity)
            {
                ModelState.AddModelError("NumberOfGuests", $"Počet hostů překračuje kapacitu pokoje ({selectedRoom.RoomType.Capacity}).");
            }

            if (ModelState.IsValid)
            {
                if (await _reservationService.IsRoomAvailableAsync(dto.RoomId, dto.CheckInDate, dto.CheckOutDate))
                {
                    await _reservationService.CreateAsync(dto);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Vybraný pokoj není v tomto termínu dostupný. Zkontrolujte rezervační štafle.");
                }
            }

            await PopulateDropdowns(dto);
            return View(dto);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var dto = await _reservationService.GetByIdForEditAsync(id.Value);
            if (dto == null) return NotFound();

            await PopulateDropdowns(dto);
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ReservationDTO dto)
        {
            if (id != dto.Id) return NotFound();

            if (dto.CheckOutDate <= dto.CheckInDate)
            {
                ModelState.AddModelError("CheckOutDate", "Datum odjezdu musí být po datu příjezdu.");
            }

            var selectedRoom = await _roomService.GetRoomForDeleteAsync(dto.RoomId);
            if (selectedRoom != null && dto.NumberOfGuests > selectedRoom.RoomType.Capacity)
            {
                ModelState.AddModelError("NumberOfGuests", $"Počet hostů překračuje kapacitu pokoje ({selectedRoom.RoomType.Capacity}).");
            }

            if (ModelState.IsValid)
            {
                if (await _reservationService.IsRoomAvailableAsync(dto.RoomId, dto.CheckInDate, dto.CheckOutDate, dto.Id))
                {
                    await _reservationService.UpdateAsync(dto);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Vybraný pokoj není v tomto termínu dostupný. Zkontrolujte rezervační štafle.");
                }
            }

            await PopulateDropdowns(dto);
            return View(dto);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var reservation = await _reservationService.GetByIdForDeleteAsync(id.Value);
            if (reservation == null) return NotFound();
            return View(reservation);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _reservationService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateDropdowns(ReservationDTO dto)
        {
            var guests = await _guestService.GetAllAsync();
            var rooms = await _roomService.GetAllAsync();
            dto.GuestsList = new SelectList(guests.Select(g => new { g.Id, FullName = $"{g.FirstName} {g.LastName}" }), "Id", "FullName", dto.GuestId);
            dto.RoomsList = new SelectList(rooms.Select(r => new { r.Id, DisplayText = $"{r.RoomNumber} ({r.RoomType.Name})" }), "Id", "DisplayText", dto.RoomId);
        }

    }
}