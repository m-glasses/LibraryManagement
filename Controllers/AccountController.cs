using LibraryManagement.ViewModels;
using LibraryManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager; 
        public AccountController(UserManager<User> userManage , SignInManager<User> signInManager)
        {
            _userManager = userManage;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel register)
        {
            if (!ModelState.IsValid)
            {
                return View(register);
            }

            var user = new User()
            {
                Name = register.Name,
                Family = register.Family,
                FatherName = register.FatherName,
                DateOfBirth = register.DateOfBirth,
                Education = register.Education.Value,
                Gender = register.Gender.Value,
                Address = register.Address,
                UserName = register.UserName,
                PhoneNumber = register.PhoneNumber,
                Email = register.Email,

            };

            var result = await _userManager.CreateAsync(user , register.Password);

            if(!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(register);
            }
            
            return RedirectToAction("Login");
            
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
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
    }
}
