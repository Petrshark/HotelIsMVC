using Microsoft.AspNetCore.Mvc;
using HotelMVCIs.Services;
using System;
using System.Threading.Tasks;

namespace HotelMVCIs.Controllers
{
    public class BookingChartController : Controller
    {
        private readonly BookingChartService _chartService;

        public BookingChartController(BookingChartService chartService)
        {
            _chartService = chartService;
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
                // Zjednodušená verze: Vždy zobrazit aktuální měsíc
                dateToShow = DateTime.Now;
            }

            var chartData = await _chartService.GetChartDataAsync(dateToShow.Year, dateToShow.Month);
            return View(chartData);
        }
    }
}