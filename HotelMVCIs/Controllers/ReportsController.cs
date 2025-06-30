using HotelMVCIs.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace HotelMVCIs.Controllers
{
    [Authorize(Roles = "Admin,Účetní")] // Přístup pouze pro role Admin a Účetní.
    public class ReportsController : Controller
    {
        private readonly PaymentService _paymentService;

        public ReportsController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // Zobrazí platební report pro zadané datumové rozmezí (nebo aktuální měsíc).
        public async Task<IActionResult> PaymentReport(DateTime? startDate, DateTime? endDate)
        {
            ViewData["Title"] = "Report plateb";

            var today = DateTime.Today;
            var start = startDate ?? new DateTime(today.Year, today.Month, 1);
            var end = endDate ?? today;

            var reportData = await _paymentService.GetPaymentReportAsync(start, end);

            return View(reportData);
        }
    }
}