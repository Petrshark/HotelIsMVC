using Microsoft.AspNetCore.Mvc;
using HotelMVCIs.Services;
using System;
using System.Threading.Tasks;

namespace HotelMVCIs.Controllers
{
    public class ReportsController : Controller
    {
        private readonly PaymentService _paymentService;

        public ReportsController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public async Task<IActionResult> PaymentReport(DateTime? startDate, DateTime? endDate)
        {
            ViewData["Title"] = "Report plateb";

            var end = endDate ?? DateTime.Today;
            var start = startDate ?? end.AddDays(-7);

            var reportData = await _paymentService.GetPaymentReportAsync(start, end);

            return View(reportData);
        }
    }
}