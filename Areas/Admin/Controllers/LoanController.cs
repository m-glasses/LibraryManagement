using LibraryManagement.Areas.Admin.ViewModels.Book;
using LibraryManagement.Areas.Admin.ViewModels.Loans;
using LibraryManagement.DTO.Book;
using LibraryManagement.DTO.Loans;
using LibraryManagement.Helper;
using LibraryManagement.Models;
using LibraryManagement.Services;
using LibraryManagement.Services.Interfaces;
using LibraryManagement.ViewModels.Loan;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Areas.Admin.Controllers
{
    [Area(Roles.Admin)]
    [Authorize(Roles = Roles.Admin)]
    public class LoanController : Controller
    {
        private readonly ILoanService _loanService;

        public LoanController(ILoanService loanService)
        {
            _loanService = loanService;
        }

        [HttpGet]
        public IActionResult Index(AdminLoanFilterViewModel inputViewModel)
        {
            ArgumentNullException.ThrowIfNull(inputViewModel);

            // Validate pagination
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

            // Validate sorting fields
            string[] allowedSortFields =
            [
                "Name",
                "Family",
                "Email",
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
                var result = _loanService.AdminGetLoansList(queryDto);

                var viewModel = new AdminLoanIndexViewModel
                {
                    Loans = result.Loans.Select(loan => new AdminLoanListViewModel
                    {
                        LoanId = loan.LoanId,
                        UserFullName = loan.UserFullName,
                        UserEmail = loan.Email,
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

            return View(new AdminLoanIndexViewModel
            {
                Loans = [],
                Filter = inputViewModel,
                TotalCount = 0,
                TotalPages = 0
            });
        }

        public IActionResult Details(int loanId)
        {
            var loan = _loanService.GetAdminDetailsById(loanId);

            if (loan is null)
                return NotFound();

            var viewModel = MapToAdminLoanDetailsViewModel(loan);

            return View(viewModel);
        }

        private AdminLoanDetailsViewModel MapToAdminLoanDetailsViewModel(Loan loan)
        {
            return new AdminLoanDetailsViewModel
            {
                // Loan details

                Id = loan.Id,
                StartDate = loan.StartDate,
                DueDate = loan.DueDate,
                ReturnDate = loan.ReturnDate,
                LoanStatus = loan.LoanStatus,
                RenewalCount = loan.RenewalCount,

                // User details

                UserId = loan.UserId,
                FullName = $"{loan.User.Name} {loan.User.Family}",

                // Book details

                BookTitle = loan.BookCopy.Book.Title,
                InventoryNumber = loan.BookCopy.InventoryNumber,

                // Financial details

                DailyRate = loan.DailyRate,
                LateFeePerDay = loan.LateFeePerDay,
                LateDays = loan.LateDays,
                CalculatedAmount = loan.CalculatedAmount,
                LateFee = loan.LateFee,
                FinalAmount = loan.FinalAmount,

                // Administrative details

                AdminNote = loan.AdminNote
            };
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmReturn(ConfirmReturnViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = ErrorMessageHelper.Translate("Invalid return date");

                return RedirectToAction(nameof(Details), new { loanId = model.LoanId });
            }

            try
            {
                _loanService.ConfirmReturn(model.LoanId, model.ReturnDate);

                TempData["SuccessMessage"] = "بازگشت امانت با موفقیت ثبت شد.";

                return RedirectToAction(nameof(Details), new { loanId = model.LoanId });
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ErrorMessageHelper.Translate(ex.Message);

                return RedirectToAction(nameof(Details), new { loanId = model.LoanId });
            }
        }
    }
}