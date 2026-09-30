using LibraryManagement.Models;
using LibraryManagement.Services;
using LibraryManagement.Services.Interfaces;
using LibraryManagement.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IdentityErrorLocalizer _identityErrorLocalizer;
        private readonly IWalletService _walletService;
        public AccountController(UserManager<User> userManage, SignInManager<User> signInManager, IdentityErrorLocalizer identityErrorLocalizer , IWalletService walletService)
        {
            _userManager = userManage;
            _signInManager = signInManager;
            _identityErrorLocalizer = identityErrorLocalizer;
            _walletService = walletService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel register)
        {
            if (!ModelState.IsValid)
            {
                return View(register);
            }

            var user = new User
            {
                Name = register.Name,
                Family = register.Family,
                FatherName = register.FatherName,
                DateOfBirth = register.DateOfBirth,
                Education = register.Education!.Value,
                Gender = register.Gender!.Value,
                Address = register.Address,
                UserName = register.UserName,
                PhoneNumber = register.PhoneNumber,
                Email = register.Email
            };

            var createResult = await _userManager.CreateAsync(
                user,
                register.Password);

            if (!createResult.Succeeded)
            {
                AddIdentityErrors(createResult);

                return View(register);
            }

            _walletService.CreateForUser(user.Id);

            var roleResult = await _userManager.AddToRoleAsync(
                user,
                "User");

            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);

                AddIdentityErrors(roleResult);

                return View(register);
            }

            return RedirectToAction(nameof(Login));
        }

        private void AddIdentityErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                var message =
                    _identityErrorLocalizer.Localizer(error.Code);

                ModelState.AddModelError("", message);
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            if (!ModelState.IsValid)
            {
                return View(login);
            }

            var result = await _signInManager.PasswordSignInAsync(
                login.UserName,
                login.Password,
                login.RememberMe,
                true
            );

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "نام کاربری یا رمزعبور اشتباه است");
                return View(login);
            }

            return RedirectToAction("Index", "Home");
        }


        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");

        }


    }
}
