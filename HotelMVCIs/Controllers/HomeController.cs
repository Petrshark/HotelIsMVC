using HotelMVCIs.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HotelMVCIs.Controllers
{
    public class HomeController : Controller
    {
        // Přesměruje na rezervační graf jako výchozí stránku.
        public IActionResult Index()
        {
            return RedirectToAction("Index", "BookingChart");
        }

        // Zobrazí stránku s ochranou osobních údajů.
        public IActionResult Privacy()
        {
            return View();
        }

        // Zobrazí chybovou stránku. Zabraňuje cachování. Předává RequestId pro ladění.
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}