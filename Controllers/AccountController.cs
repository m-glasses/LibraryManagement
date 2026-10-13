
using LibraryManagement.DTO;
using LibraryManagement.Exceptions;
using LibraryManagement.Models;
using LibraryManagement.Services.Interfaces;
using LibraryManagement.ViewModels;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<User> _signInManager;
    private readonly IAccountService _accountService;

    public AccountController(
        SignInManager<User> signInManager,
        IAccountService accountService)
    {
        _signInManager = signInManager;
        _accountService = accountService;
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
            return View(register);

        var dto = new RegisterDto
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
            Email = register.Email,
            Password = register.Password
        };

        try
        {
            await _accountService.RegisterAsync(dto);

            return RedirectToAction(nameof(Login));
        }
        catch (RegistrationException ex)
        {
            foreach (var error in ex.Errors)
                ModelState.AddModelError(string.Empty, error);

            return View(register);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(
                string.Empty,
                Helper.ErrorMessageHelper.Translate(ex.Message));

            return View(register);
        }
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel login)
    {
        if (!ModelState.IsValid)
            return View(login);

        var dto = new LoginDto()
        {
            UserName = login.UserName,
            Password = login.Password,
            RememberMe = login.RememberMe
        };

        try
        {
            await _accountService.LoginAsync(dto);

            return RedirectToAction("Index", "Home");
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(
                string.Empty,
                Helper.ErrorMessageHelper.Translate(ex.Message));

            return View(login);
        }
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();

        return RedirectToAction(nameof(Login));
    }
}
