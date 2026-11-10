using LibraryManagement.Areas.Admin.ViewModels.Loans;
using LibraryManagement.DTO.Loans;
using LibraryManagement.Helper;
using LibraryManagement.Models;
using LibraryManagement.Services.Interfaces;
using LibraryManagement.ViewModels;
using LibraryManagement.ViewModels.Loan;
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
                TempData["ErrorMessage"] = ErrorMessageHelper.Translate(ex.Message);

                return RedirectToAction(
                    nameof(BookController.Details),
                    "Book",
                    new { id = bookId });
            }
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> MyLoans(LoanFilterViewModel inputViewModel)
        {
            ArgumentNullException.ThrowIfNull(inputViewModel);

            var user = await _userManager.GetUserAsync(User);

            if (user is null)
            {
                return Unauthorized();
            }

            // 1. Validate pagination.
            if (inputViewModel.Page < 1)
            {
                inputViewModel.Page = 1;
                ModelState.Remove(nameof(inputViewModel.Page));
            }

            if (inputViewModel.PageSize is not (10 or 20 or 50))
            {
                inputViewModel.PageSize = 20;
                ModelState.Remove(nameof(inputViewModel.PageSize));
            }

            // 2. Validate sorting fields.
            string[] allowedSortFields =
            [
                "Title",
                "Author",
                "InventoryNumber",
                "StartDate",
                "DueDate",
                "ReturnDate"
            ];

            if (!string.IsNullOrWhiteSpace(inputViewModel.SortBy) &&
                !allowedSortFields.Contains(
                    inputViewModel.SortBy,
                    StringComparer.OrdinalIgnoreCase))
            {
                inputViewModel.SortBy = null;
                ModelState.Remove(nameof(inputViewModel.SortBy));
            }

            // 3. Map ViewModel to DTO.
            var queryDto = new LoanFilterDto
            {
                SearchTerm = inputViewModel.SearchTerm?.Trim(),
                Status = inputViewModel.Status,

                StartDateFrom = inputViewModel.StartDateFrom,
                StartDateTo = inputViewModel.StartDateTo,

                DueDateFrom = inputViewModel.DueDateFrom,
                DueDateTo = inputViewModel.DueDateTo,

                ReturnDateFrom = inputViewModel.ReturnDateFrom,
                ReturnDateTo = inputViewModel.ReturnDateTo,

                SortBy = inputViewModel.SortBy,
                SortDescending = inputViewModel.SortDescending,

                Page = inputViewModel.Page,
                PageSize = inputViewModel.PageSize
            };

            try
            {
                // 4. Get current user's loans.
                var result = _loanService.UserGetLoansList(user.Id, queryDto);

                // 5. Map DTO result to ViewModel.
                var viewModel = new LoanIndexViewModel
                {
                    Loans = result.Loans.Select(loan => new LoanListViewModel
                    {
                        LoanId = loan.LoanId,
                        BookTitle = loan.BookTitle,
                        Author = loan.Author,
                        InventoryNumber = loan.InventoryNumber,
                        StartDate = loan.StartDate,
                        DueDate = loan.DueDate,
                        ReturnDate = loan.ReturnDate,
                        Status = loan.Status,
                        FinalAmount = loan.FinalAmount
                    }).ToList(),

                    Filter = inputViewModel,
                    TotalCount = result.TotalCount,
                    TotalPages = result.TotalPages
                };

                return View(viewModel);
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = ErrorMessageHelper.Translate(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ErrorMessageHelper.Translate(ex.Message);
            }

            // 6. Return an empty result when a handled error occurs.
            return View(new LoanIndexViewModel
            {
                Loans = [],
                Filter = inputViewModel,
                TotalCount = 0,
                TotalPages = 0
            });
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

            try
            {
                _loanService.RequestReturn(loanId, user.Id);

                return RedirectToAction(nameof(MyLoans));
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ErrorMessageHelper.Translate(ex.Message);

                return RedirectToAction(nameof(MyLoans));
            }
        }



        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Renew(int loanId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
            {
                return Unauthorized();
            }

            try
            {
                _loanService.Renew(loanId, user.Id);

                TempData["SuccessMessage"] = "امانت با موفقیت تمدید شد.";

                return RedirectToAction(
                    nameof(Details),
                    new { id = loanId });

            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ErrorMessageHelper.Translate(ex.Message);

                return RedirectToAction(nameof(Details), new { id = loanId });
            }
        }



        //[Authorize(Roles = "Admin")]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ConfirmReturn(ConfirmReturnViewModel confirm)
        //{
        //    try
        //    {
        //        _loanService.ConfirmReturn(confirm.LoanId, confirm.ReturnDate);
        //        TempData["SuccessMessage"] = ErrorMessageHelper.Translate("تایید بازگشت امانت با موفقیت انجام شد");
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        TempData["ErrorMessage"] = ErrorMessageHelper.Translate(ex.Message);
        //        return RedirectToAction()
        //    }

        //}

    }
}
