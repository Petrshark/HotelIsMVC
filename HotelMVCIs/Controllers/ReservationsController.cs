using Microsoft.AspNetCore.Mvc;
using HotelMVCIs.Services;
using HotelMVCIs.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;
using HotelMVCIs.ViewModels;
using HotelMVCIs.Data;
using Microsoft.EntityFrameworkCore;
using HotelMVCIs.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;

namespace HotelMVCIs.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly ReservationService _reservationService;
        private readonly GuestService _guestService;
        private readonly RoomService _roomService;
        private readonly HotelServiceService _hotelServiceService;
        private readonly PaymentService _paymentService;
        private readonly HotelMVCIsDbContext _context;

        public ReservationsController(
            ReservationService reservationService,
            GuestService guestService,
            RoomService roomService,
            HotelServiceService hotelServiceService,
            PaymentService paymentService,
            HotelMVCIsDbContext context)
        {
            _reservationService = reservationService;
            _guestService = guestService;
            _roomService = roomService;
            _hotelServiceService = hotelServiceService;
            _paymentService = paymentService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var reservations = await _reservationService.GetAllAsync();
            return View(reservations);
        }

        // GET: Reservations/Create
        // Metoda nyní přijímá volitelné parametry z rezervační tabule
        public async Task<IActionResult> Create(int? roomId, DateTime? checkInDate)
        {
            var dto = new ReservationDTO();

            // Pokud jsme dostali parametry z odkazu, předvyplníme je do DTO
            if (roomId.HasValue)
            {
                dto.RoomId = roomId.Value;
            }
            if (checkInDate.HasValue)
            {
                dto.CheckInDate = checkInDate.Value;
                dto.CheckOutDate = checkInDate.Value.AddDays(1); // Nastavíme odjezd na další den
            }

            await PopulateDropdowns(dto);
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReservationDTO dto)
        {
            var selectedRoom = await _roomService.GetRoomForDeleteAsync(dto.RoomId);
            if (selectedRoom != null && dto.NumberOfGuests > selectedRoom.RoomType.Capacity)
            {
                ModelState.AddModelError("NumberOfGuests", $"Počet hostů překračuje kapacitu pokoje ({selectedRoom.RoomType.Capacity}).");
            }
            if (dto.CheckOutDate <= dto.CheckInDate)
            {
                ModelState.AddModelError("CheckOutDate", "Datum odjezdu musí být po datu příjezdu.");
            }
            if (ModelState.IsValid)
            {
                if (await _reservationService.IsRoomAvailableAsync(dto.RoomId, dto.CheckInDate, dto.CheckOutDate))
                {
                    int newReservationId = await _reservationService.CreateAsync(dto);
                    return RedirectToAction(nameof(Edit), new { id = newReservationId });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Vybraný pokoj není v tomto termínu dostupný.");
                }
            }
            await PopulateDropdowns(dto);
            return View(dto);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var reservation = await _context.Reservations
                .Include(r => r.ReservationItems)
                    .ThenInclude(ri => ri.HotelService)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id.Value);

            if (reservation == null) return NotFound();

            var reservationDto = await _reservationService.GetByIdForEditAsync(id.Value);

            var allServices = await _hotelServiceService.GetAllAsync();
            var availableServices = allServices
                                .Where(s => s.IsActive)
                                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = $"{s.Name} ({s.Price:C})" });

            var accommodationPrice = reservation.TotalPrice;
            var servicesPrice = reservation.ReservationItems.Sum(ri => ri.Quantity * ri.PricePerItem);
            var grandTotal = accommodationPrice + servicesPrice;
            var totalPaid = await _paymentService.GetTotalPaidForReservationAsync(id.Value);
            var remainingBalance = grandTotal - totalPaid;

            var viewModel = new ReservationEditViewModel
            {
                Reservation = reservationDto,
                AddedServices = reservation.ReservationItems.ToList(),
                AvailableServices = new SelectList(availableServices, "Value", "Text"),
                AccommodationPrice = accommodationPrice,
                ServicesPrice = servicesPrice,
                GrandTotal = grandTotal,
                TotalPaid = totalPaid,
                RemainingBalance = remainingBalance
            };

            await PopulateDropdowns(viewModel.Reservation);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind(Prefix = "Reservation")] ReservationDTO reservationDto)
        {
            if (id != reservationDto.Id) return NotFound();

            // Zde by měla být plná validace, jako v Create akci
            if (ModelState.IsValid)
            {
                try
                {
                    await _reservationService.UpdateAsync(reservationDto);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _reservationService.ExistsAsync(id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Edit), new { id = reservationDto.Id });
            }
            return await Edit(id); // Znovu načteme a zobrazíme stránku s chybami
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddServiceToReservation(int reservationId, int serviceToAddId, int serviceToAddQuantity)
        {
            if (serviceToAddQuantity < 1)
            {
                TempData["ErrorMessage"] = "Počet musí být alespoň 1.";
                return RedirectToAction(nameof(Edit), new { id = reservationId });
            }
            var service = await _hotelServiceService.GetByIdAsync(serviceToAddId);
            if (service == null) return NotFound();

            var existingItem = await _context.ReservationItems.FirstOrDefaultAsync(ri => ri.ReservationId == reservationId && ri.HotelServiceId == serviceToAddId);
            if (existingItem != null)
            {
                existingItem.Quantity += serviceToAddQuantity;
            }
            else
            {
                _context.ReservationItems.Add(new ReservationItem
                {
                    ReservationId = reservationId,
                    HotelServiceId = serviceToAddId,
                    Quantity = serviceToAddQuantity,
                    PricePerItem = service.Price
                });
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Edit), new { id = reservationId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveServiceFromReservation(int reservationId, int hotelServiceId)
        {
            var itemToRemove = await _context.ReservationItems.FirstOrDefaultAsync(ri => ri.ReservationId == reservationId && ri.HotelServiceId == hotelServiceId);
            if (itemToRemove != null)
            {
                _context.ReservationItems.Remove(itemToRemove);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Edit), new { id = reservationId });
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