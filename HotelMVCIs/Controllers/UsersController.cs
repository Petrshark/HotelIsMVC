using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using HotelMVCIs.Models;
using HotelMVCIs.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace HotelMVCIs.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IPasswordHasher<AppUser> _passwordHasher;
        private readonly IPasswordValidator<AppUser> _passwordValidator;

        public UsersController(
            UserManager<AppUser> userManager,
            IPasswordHasher<AppUser> passwordHasher,
            IPasswordValidator<AppUser> passwordValidator)
        {
            _userManager = userManager;
            _passwordHasher = passwordHasher;
            _passwordValidator = passwordValidator;
        }

        public IActionResult Index() => View(_userManager.Users.ToList());

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserVM userVM)
        {
            if (ModelState.IsValid)
            {
                AppUser appUser = new AppUser
                {
                    UserName = userVM.Email,
                    Email = userVM.Email,
                    Name = userVM.Name
                };

                IdentityResult result = await _userManager.CreateAsync(appUser, userVM.Password);
                if (result.Succeeded) return RedirectToAction("Index");
                else AddErrorsFromResult(result);
            }
            return View(userVM);
        }

        public async Task<IActionResult> Edit(string id)
        {
            AppUser user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var vm = new UserEditVM
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email
                };
                return View(vm);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEditVM userVM)
        {
            AppUser user = await _userManager.FindByIdAsync(userVM.Id);
            if (user != null)
            {
                if (!string.IsNullOrEmpty(userVM.Email)) user.Email = userVM.Email;
                if (!string.IsNullOrEmpty(userVM.Name)) user.Name = userVM.Name;

                // UserName must always be in sync with Email for login to work
                user.UserName = userVM.Email;

                if (!string.IsNullOrEmpty(userVM.Password))
                {
                    IdentityResult passwordResult = await _passwordValidator.ValidateAsync(_userManager, user, userVM.Password);
                    if (passwordResult.Succeeded)
                    {
                        user.PasswordHash = _passwordHasher.HashPassword(user, userVM.Password);
                    }
                    else
                    {
                        AddErrorsFromResult(passwordResult);
                    }
                }

                if (ModelState.ErrorCount == 0)
                {
                    IdentityResult result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded) return RedirectToAction("Index");
                    else AddErrorsFromResult(result);
                }
            }
            else
            {
                ModelState.AddModelError("", "User Not Found");
            }
            return View(userVM);
        }

        public async Task<IActionResult> Delete(string id)
        {
            AppUser user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            else return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            AppUser user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                if (user.UserName == User.Identity.Name)
                {
                    ModelState.AddModelError("", "Nemůžete smazat sám sebe.");
                    return View("Index", _userManager.Users.ToList());
                }
                IdentityResult result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded) AddErrorsFromResult(result);
            }
            return RedirectToAction(nameof(Index));
        }

        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }
    }
}