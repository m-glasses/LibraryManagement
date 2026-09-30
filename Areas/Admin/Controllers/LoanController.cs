using LibraryManagement.Models;
using LibraryManagement.Services.Interfaces;
using LibraryManagement.ViewModels;
using LibraryManagement.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class LoanController : Controller
    {
        private readonly ILoanService _loanService;

        public LoanController(ILoanService loanService)
        {
            _loanService = loanService;
        }

        public IActionResult Index()
        {
            var loans = _loanService.GetAllLoans();

            List<AdminLoanListViewModel> viewModels = loans
                .Select(loan => new AdminLoanListViewModel
                {
                    Id = loan.Id,
                    UserFullName = $"{loan.User.Name} {loan.User.Family}",
                    BookTitle = loan.BookCopy.Book.Title,
                    InventoryNumber = loan.BookCopy.InventoryNumber,
                    LoanDate = loan.StartDate,
                    DueDate = loan.DueDate,
                    ReturnDate = loan.ReturnDate,
                    LoanStatus = loan.LoanStatus,
                    FinalAmount = loan.FinalAmount
                })
                .ToList();

            return View(viewModels);
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

                return RedirectToAction(nameof(Details),new { loanId = model.LoanId });
            }

            try
            {
                _loanService.ConfirmReturn(model.LoanId,model.ReturnDate);

                TempData["SuccessMessage"] = "بازگشت امانت با موفقیت ثبت شد.";

                return RedirectToAction(nameof(Details), new { loanId = model.LoanId });
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ErrorMessageHelper.Translate(ex.Message);

                return RedirectToAction( nameof(Details), new { loanId = model.LoanId });
            }
        }
    }
}