using LibraryManagement.Models;
using LibraryManagement.Services;
using LibraryManagement.Services.Interfaces;
using LibraryManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    public class LoanController : Controller
    {
        private readonly ILoanService _loanService;
        private readonly IBookCopyService _bookCopyService;
        private readonly UserManager<User> _userManager;

        public LoanController(ILoanService loanService, UserManager<User> userManager, IBookCopyService bookCopyService)
        {
            _loanService = loanService;
            _userManager = userManager;
            _bookCopyService = bookCopyService;

        }

        // GET: LoanController
        public IActionResult Index()
        {
            var loans = _loanService.GetAll();

            if (loans == null)
            {
                return NotFound();
            }

            return View(loans);
        }


        private LoanDetailsViewModel MapToDetailsViewModel(Loan loan)
        {
            return new LoanDetailsViewModel
            {
                Id = loan.Id,
                BookTitle = loan.BookCopy.Book.Title,
                Author = loan.BookCopy.Book.Author,
                InventoryNumber = loan.BookCopy.InventoryNumber,
                StartDate = loan.StartDate,
                DueDate = loan.DueDate,
                LoanStatus = loan.LoanStatus
            };
        }

        // GET: LoanController/Details/5

        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var loan = _loanService.GetDetailsById(id, user.Id);

            if (loan == null)
            {
                return NotFound();
            }

            var viewModel = MapToDetailsViewModel(loan);

            return View(viewModel);
        }

        public IActionResult Borrow()
        {

            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Borrow(int bookId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
            {
                return Unauthorized();
            }

            try
            {
                var loan = _loanService.Borrow(user.Id, bookId);

                return RedirectToAction(
                    nameof(Details),
                    new { id = loan.Id });
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;

                return RedirectToAction(
                    nameof(BookController.Details),
                    "Book",
                    new { id = bookId });
            }
        }

        [Authorize]
        public async Task<IActionResult> MyLoans()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized();
            }

            var userLoans = _loanService.GetUserLoans(user.Id);

            var viewModels = userLoans
                .Select(loan => new LoanListViewModel
                {
                    Id = loan.Id,
                    BookTitle = loan.BookCopy.Book.Title,
                    Author = loan.BookCopy.Book.Author,
                    StartDate = loan.StartDate,
                    DueDate = loan.DueDate,
                    LoanStatus = loan.LoanStatus
                })
                .ToList();

            return View(viewModels);
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestReturn(int loanId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
            {
                return Unauthorized();
            }

            if (!_loanService.RequestReturn(loanId, user.Id))
            {
                return NotFound();
            }

            return RedirectToAction(nameof(MyLoans));
        }

    }
}
