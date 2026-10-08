using LibraryManagement.Models;
using LibraryManagement.Areas.Admin.ViewModels;
using LibraryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LibraryManagement.DTO;
using LibraryManagement.Helper;

namespace LibraryManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;
        public UserController(IUserService userService , UserManager<User> usermanager)
        {
            _userService = userService;
            _userManager = usermanager;
        }


        public IActionResult Index(AdminUserFilterViewModel inputViewModel)
        {
            var queryDto = new UserListQueryDto
            {
                SearchTerm = inputViewModel.SearchTerm,
                Role = inputViewModel.Role,
                IsDebtor = inputViewModel.IsDebtor,
                Page = inputViewModel.Page,
                PageSize = inputViewModel.PageSize
            };

            var result = _userService.GetAllUsers(queryDto);

            var viewModel = new AdminUserIndexViewModel
            {
                Users = result.Users
                    .Select(user => new UserListViewModel
                    {
                        Id = user.Id,
                        FullName = $"{user.Name} {user.Family}",
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        Role = user.Role,
                        IsDebtor = user.WalletBalance < 0,
                        WalletBalance = user.WalletBalance
                    })
                    .ToList(),

                SearchTerm = inputViewModel.SearchTerm,
                Role = inputViewModel.Role,
                IsDebtor = inputViewModel.IsDebtor,

                CurrentPage = result.CurrentPage,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount,
                TotalPages = result.TotalPages
            };

            return View(viewModel);
        }


        public IActionResult Details(int id)
        {
            var dto = _userService.GetUserDetails(id);

            if (dto is null)
            {
                return NotFound();
            }

            var viewModel = new AdminUserDetailsViewModel
            {
                Id = dto.Id,
                FullName = $"{dto.Name} {dto.Family}",
                FatherName = dto.FatherName,
                UserName = dto.UserName,
                Email = dto.Email,
                DateOfBirth = dto.DateOfBirth,
                Gender = dto.Gender,
                Education = dto.Education,
                PhoneNumber = dto.PhoneNumber,
                Role = dto.Role,
                Address = dto.Address,
                WalletBalance = dto.WalletBalance,
                IsDebtor = dto.WalletBalance < 0,
                IsActive = dto.IsActive
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Deactivate(int id)
        {
            try
            {
                _userService.Deactivate(id);

                TempData["SuccessMessage"] = "کاربر با موفقیت غیرفعال شد";

                return RedirectToAction(nameof(Details), new { id });
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ErrorMessageHelper.Translate(ex.Message);

                return RedirectToAction(nameof(Details), new { id });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Activate(int id)
        {
            try
            {
                _userService.Activate(id);

                TempData["SuccessMessage"] = "کاربر با موفقیت فعال شد";

                return RedirectToAction(nameof(Details), new { id });
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ErrorMessageHelper.Translate(ex.Message);

                return RedirectToAction(nameof(Details), new { id });
            }
        }

        public IActionResult Edit(int id)
        {
            var user = _userService.GetUserDetails(id);

            if (user is null)
            {
                return NotFound();
            }

            var viewModel = new AdminUserEditViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Family = user.Family,
                FatherName = user.FatherName,
                DateOfBirth = user.DateOfBirth,
                Education = user.Education,
                Gender = user.Gender,
                Address = user.Address,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AdminUserEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var dto = new AdminUserUpdateDto()
                {
                    Id = model.Id,
                    Name = model.Name,
                    Family = model.Family,
                    FatherName = model.FatherName,
                    DateOfBirth = model.DateOfBirth,
                    Education = model.Education,
                    Gender = model.Gender,
                    Address = model.Address,
                    UserName = model.UserName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Role = model.Role
                };

                await _userService.UpdateUserAsync(dto);

                return RedirectToAction(nameof(Details), new { model.Id });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(
                    string.Empty,
                    ErrorMessageHelper.Translate(ex.Message));

                return View(model);
            }
        }
    }
}
