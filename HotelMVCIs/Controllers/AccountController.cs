using HotelMVCIs.Models;
using HotelMVCIs.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HotelMVCIs.Controllers
{
    [Authorize] // Vyžaduje přihlášení pro většinu akcí v tomto kontroleru.
    public class AccountController : Controller
    {
        // Injektované služby z ASP.NET Core Identity pro správu uživatelů a přihlášení.
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // Akce pro zobrazení přihlašovacího formuláře (GET). Přístupná i nepřihlášeným.
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl)
        {
            return View(new LoginVM
            {
                ReturnUrl = returnUrl ?? "/" // Předává URL pro přesměrování po přihlášení.
            });
        }

        // Akce pro zpracování odeslaného přihlašovacího formuláře (POST).
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken] // Ochrana před CSRF útoky.
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (ModelState.IsValid) // Kontroluje platnost dat z formuláře.
            {
                AppUser? user = await _userManager.FindByEmailAsync(loginVM.Email);
                if (user != null)
                {
                    await _signInManager.SignOutAsync(); // Odhlásí předchozí sezení.
                    var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, loginVM.RememberMe, false);
                    if (result.Succeeded) // Pokud je přihlášení úspěšné.
                    {
                        return LocalRedirect(loginVM.ReturnUrl ?? "/"); // Přesměruje uživatele.
                    }
                }
                ModelState.AddModelError("", "Neplatné přihlašovací údaje."); // Chyba pro neúspěšné přihlášení.
            }
            return View(loginVM); // Vrátí formulář s chybami.
        }

        // Akce pro odhlášení uživatele (POST).
        [HttpPost]
        [ValidateAntiForgeryToken] // Ochrana před CSRF útoky.
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync(); // Provede odhlášení.
            return RedirectToAction("Index", "Home"); // Přesměruje na domovskou stránku.
        }

        // Akce pro zobrazení stránky "Přístup odepřen". Přístupná i nepřihlášeným.
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}