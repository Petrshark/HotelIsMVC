using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotelMVCIs.Data;
using HotelMVCIs.DTOs;
using HotelMVCIs.Models;
using HotelMVCIs.Services;
using HotelMVCIs.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace HotelMVCIs.Controllers
{
    [Authorize(Roles = "Admin,Recepční")]
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
            return View(await _reservationService.GetAllAsync());
        }

        public async Task<IActionResult> Create(int? roomId, System.DateTime? checkInDate)
        {
            var dto = new ReservationDTO();
            if (roomId.HasValue) dto.RoomId = roomId.Value;
            if (checkInDate.HasValue)
            {
                dto.CheckInDate = checkInDate.Value;
                dto.CheckOutDate = checkInDate.Value.AddDays(1);
            }
            await PopulateDropdowns(dto);
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReservationDTO dto)
        {
            var room = await _context.Rooms.Include(r => r.RoomType).FirstOrDefaultAsync(r => r.Id == dto.RoomId);
            if (room != null && dto.NumberOfGuests > room.RoomType.Capacity)
            {
                ModelState.AddModelError("NumberOfGuests", $"Počet hostů překračuje kapacitu pokoje ({room.RoomType.Capacity}).");
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
                .Include(r => r.ReservationItems).ThenInclude(ri => ri.HotelService)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id.Value);

            if (reservation == null) return NotFound();

            var reservationDto = await _reservationService.GetByIdAsync(id.Value);
            if (reservationDto == null) return NotFound();

            var allServices = await _hotelServiceService.GetAllAsync();
            var availableServices = allServices
                .Where(s => s.IsActive && !reservation.ReservationItems.Any(rs => rs.HotelServiceId == s.Id))
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
            if (ModelState.IsValid)
            {
                await _reservationService.UpdateAsync(reservationDto);
                return RedirectToAction(nameof(Edit), new { id = reservationDto.Id });
            }
            return await Edit(id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddServiceToReservation(int reservationId, int serviceToAddId, int serviceToAddQuantity)
        {
            var service = await _context.HotelServices.FindAsync(serviceToAddId);
            if (service == null) return NotFound();

            var existingItem = await _context.ReservationItems.FirstOrDefaultAsync(ri => ri.ReservationId == reservationId && ri.HotelServiceId == serviceToAddId);
            if (existingItem != null)
            {
                existingItem.Quantity += serviceToAddQuantity > 0 ? serviceToAddQuantity : 1;
            }
            else
            {
                _context.ReservationItems.Add(new ReservationItem
                {
                    ReservationId = reservationId,
                    HotelServiceId = serviceToAddId,
                    Quantity = serviceToAddQuantity > 0 ? serviceToAddQuantity : 1,
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
            dto.GuestsList = await _guestService.GetAllAsync().ContinueWith(t => t.Result.Select(g => new SelectListItem { Value = g.Id.ToString(), Text = $"{g.FirstName} {g.LastName}" }));
            dto.RoomsList = await _roomService.GetAllAsync().ContinueWith(t => t.Result.Select(r => new SelectListItem { Value = r.Id.ToString(), Text = $"{r.RoomNumber} ({r.RoomType.Name})" }));
        }
    }
}