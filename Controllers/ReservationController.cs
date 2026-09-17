using LibraryManagement.Models;
using LibraryManagement.Services;
using LibraryManagement.Services.Interfaces;
using LibraryManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LibraryManagement.Helper;

namespace LibraryManagement.Controllers
{
    public class ReservationController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IReservationService _reservationService;
        public ReservationController(UserManager<User> userManager, IReservationService reservationService)
        {
            _reservationService = reservationService;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        private static ReservationDetailsViewModel MapToDetailsViewModel(Reservation reservation)
        {
            return new ReservationDetailsViewModel
            {
                Id = reservation.Id,
                BookId = reservation.BookId,
                BookTitle = reservation.Book.Title,
                ReservedAt = reservation.ReservedAt,
                ExpiresAt = reservation.ExpiresAt,
                ReservationStatus = reservation.ReservationStatus
            };
        }

        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
            {
                return Unauthorized();
            }

            var reservation = _reservationService.GetDetailsById(id, user.Id);

            if (reservation is null)
            {
                return NotFound();
            }

            var viewModel = MapToDetailsViewModel(reservation);

            return View(viewModel);
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reserve(int bookId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
            {
                return Unauthorized();
            }

            try
            {
                var reservation = _reservationService.Reserve(user.Id, bookId);

                return RedirectToAction(
                    nameof(Details),
                    new { id = reservation.Id });
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ErrorMessageHelper.Translate(ex.Message);

                return RedirectToAction(
                    nameof(BookController.Details),
                    "Book",
                    new { id = bookId });
            }
        }




        [Authorize]
        public async Task<IActionResult> MyReservations()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
            {
                return Unauthorized();
            }

            var userReservations =
                _reservationService.GetUserReservations(user.Id);

            var viewModel = userReservations
                .Select(reservation => new ReservationListViewModel
                {
                    Id = reservation.Id,
                    BookTitle = reservation.Book.Title,
                    ReservedAt = reservation.ReservedAt,
                    ExpiresAt = reservation.ExpiresAt,
                    ReservationStatus = reservation.ReservationStatus
                })
                .ToList();

            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelReservation(int reservationId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
            {
                return Unauthorized();
            }

            try
            {
                _reservationService.CancelReservation(
                    reservationId,
                    user.Id);

                return RedirectToAction(nameof(MyReservations));
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;

                return RedirectToAction(
                    nameof(MyReservations));
            }
        }
    }
}
