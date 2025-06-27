using HotelMVCIs.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HotelMVCIs.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Hlavní stránka přesměrovává na nejdůležitější pohled - rezervační tabuli
            return RedirectToAction("Index", "BookingChart");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}