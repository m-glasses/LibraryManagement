using LibraryManagement.Areas.Admin.ViewModels.Book;
using LibraryManagement.Areas.Admin.ViewModels.BookCopies;
using LibraryManagement.DTO.Book;
using LibraryManagement.DTO.BookCopies;
using LibraryManagement.Helper;
using LibraryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Areas.Admin.Controllers
{
    [Area(Roles.Admin)]
    [Authorize(Roles = Roles.Admin)]
    public class BookCopyController : Controller
    {
        private readonly IBookCopyService _bookCopyServices;
        public BookCopyController(IBookCopyService bookCopyService)
        {
            _bookCopyServices = bookCopyService;
        }


        #region Index
        public IActionResult Index(AdminBookCopiesFilterViewModel inputViewModel)
        {
            ArgumentNullException.ThrowIfNull(inputViewModel);

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

            string[] allowedSortFields =
            [
                "Title",
                "Author",
                "Publisher",
                "Status",
                "InventoryNumber"
            ];

            if (!string.IsNullOrWhiteSpace(inputViewModel.SortBy) &&
                !allowedSortFields.Contains(
                    inputViewModel.SortBy,
                    StringComparer.OrdinalIgnoreCase))
            {
                inputViewModel.SortBy = null;
                ModelState.Remove(nameof(inputViewModel.SortBy));
            }

            if (inputViewModel.Status.HasValue &&
                !Enum.IsDefined(inputViewModel.Status.Value))
            {
                inputViewModel.Status = null;
                ModelState.Remove(nameof(inputViewModel.Status));
            }

            var queryDto = new BookCopiesFilterDto
            {
                SearchTerm = inputViewModel.SearchTerm?.Trim(),
                Status = inputViewModel.Status,
                SortBy = inputViewModel.SortBy,
                SortDescending = inputViewModel.SortDescending,
                Page = inputViewModel.Page,
                PageSize = inputViewModel.PageSize
            };

            try
            {
                var result = _bookCopyServices.GetBookCopiesList(queryDto);

                var viewModel = new AdminBookCopiesIndexViewModel
                {
                    BookCopies = result.BookCopies
                        .Select(bookCopy => new AdminBookCopiesListViewModel
                        {
                            Id = bookCopy.Id,
                            BookId = bookCopy.BookID,
                            Title = bookCopy.Title,
                            Author = bookCopy.Author,
                            Publisher = bookCopy.Publisher,
                            InventoryNumber = bookCopy.InventoryNumber,
                            Status = bookCopy.Status,
                            LastBorrowerName = bookCopy.LastBorrowerName,
                            LastLoanStartDate = bookCopy.LastLoanStartDate,
                            LastLoanDueDate = bookCopy.LastLoanDueDate
                        })
                        .ToList(),

                    Filter = inputViewModel,
                    TotalCount = result.TotalCount,
                    TotalPages = result.TotalPages
                };

                return View(viewModel);
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] =
                    ErrorMessageHelper.Translate(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] =
                    ErrorMessageHelper.Translate(ex.Message);
            }

            return View(new AdminBookCopiesIndexViewModel
            {
                BookCopies = [],
                Filter = inputViewModel,
                TotalCount = 0,
                TotalPages = 0
            });
        }

        #endregion Index
    }
}
