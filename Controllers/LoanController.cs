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
        public IActionResult Details(int id)
        {
            var loan = _loanService.GetDetailsById(id);

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

            if (user == null)
            {
                return Unauthorized();
            }

            var bookCopy = _loanService.FindAvailableBookCopy(bookId);

            var loan = _loanService.Borrow(user.Id, bookCopy.Id);

            return RedirectToAction(nameof(Details), "Loan", new { id = bookId });
        }


    }
}
