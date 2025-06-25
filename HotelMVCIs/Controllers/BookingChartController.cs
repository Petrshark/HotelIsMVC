using Microsoft.AspNetCore.Mvc;
using HotelMVCIs.Services;
using HotelMVCIs.Models;

namespace HotelMVCIs.Controllers
{
    public class BookingChartController : Controller
    {
        private readonly BookingChartService _chartService;
        private readonly ReservationService _reservationService;

        public BookingChartController(BookingChartService chartService, ReservationService reservationService)
        {
            _chartService = chartService;
            _reservationService = reservationService;
        }

        public async Task<IActionResult> Index(int? year, int? month)
        {
            DateTime dateToShow;

            if (year.HasValue && month.HasValue)
            {
                dateToShow = new DateTime(year.Value, month.Value, 1);
            }
            else
            {
                var earliestDate = await _reservationService.GetEarliestReservationDateAsync();
                dateToShow = earliestDate ?? DateTime.Now;
            }

            var chartData = await _chartService.GetChartDataAsync(dateToShow.Year, dateToShow.Month);
            return View(chartData);
        }
    }
}