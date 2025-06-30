using HotelMVCIs.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace HotelMVCIs.Controllers
{
    [Authorize] // Vyžaduje přihlášení pro přístup k rezervačnímu grafu.
    public class BookingChartController : Controller
    {
        private readonly BookingChartService _chartService; // Injektovaná služba pro data grafu.

        public BookingChartController(BookingChartService chartService)
        {
            _chartService = chartService;
        }

        // Akce pro zobrazení hlavního rezervačního grafu (GET).
        // Přijímá volitelné parametry 'year' a 'month' pro navigaci mezi měsíci.
        public async Task<IActionResult> Index(int? year, int? month)
        {
            // Určí datum pro zobrazení grafu (aktuální měsíc, pokud není zadán).
            DateTime dateToShow = (year.HasValue && month.HasValue)
                ? new DateTime(year.Value, month.Value, 1)
                : DateTime.Today;

            // Získá data pro graf ze služby.
            var chartData = await _chartService.GetBookingChartAsync(dateToShow);

            return View(chartData); // Předá data pohledu.
        }
    }
}