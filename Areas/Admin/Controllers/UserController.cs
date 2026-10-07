using LibraryManagement.Models;
using LibraryManagement.Areas.Admin.ViewModels;
using LibraryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LibraryManagement.DTO;

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
                Email = dto.Email,
                DateOfBirth = dto.DateOfBirth,
                Gender = dto.Gender,
                Education = dto.Education,
                PhoneNumber = dto.PhoneNumber,
                Role = dto.Role,
                Address = dto.Address,
                WalletBalance = dto.WalletBalance,
                IsDebtor = dto.WalletBalance < 0
            };

            return View(viewModel);
        }
    }
}
