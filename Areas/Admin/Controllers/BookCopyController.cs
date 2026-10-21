using LibraryManagement.Areas.Admin.ViewModels.Book;
using LibraryManagement.Areas.Admin.ViewModels.BookCopies;
using LibraryManagement.DTO.Book;
using LibraryManagement.DTO.Book.Admin;
using LibraryManagement.DTO.BookCopies;
using LibraryManagement.Helper;
using LibraryManagement.Models;
using LibraryManagement.Services;
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
        private readonly IBookService _bookService;
        public BookCopyController(IBookCopyService bookCopyService , IBookService bookService)
        {
            _bookCopyServices = bookCopyService;
            _bookService = bookService;
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
                BookId = inputViewModel.BookId,
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

        #region Details

        public IActionResult Details(int bookCopyId)
        {
            if (bookCopyId <= 0)
            {
                return NotFound();
            }

            var dto = _bookCopyServices.GetBookCopyDetails(bookCopyId);

            if (dto is null)
            {
                return NotFound();
            }

            var viewModel = new AdminBookCopyDetailsViewModel
            {
                Id = dto.Id,
                BookId = dto.BookId,
                Title = dto.Title,
                Author = dto.Author,
                Publisher = dto.Publisher,
                PublicationYear = dto.PublicationYear,
                PublicationSeason = dto.PublicationSeason,
                PageCount = dto.PageCount,
                Edition = dto.Edition,
                Volume = dto.Volume,
                InventoryNumber= dto.InventoryNumber,
                Status = dto.Status,
                LastBorrowerName = dto.LastBorrowerName,
                LastLoanDueDate = dto.LastLoanDueDate,
                LastLoanStartDate = dto.LastLoanStartDate,
            };

            return View(viewModel);
        }

        #endregion

        #region Edit

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var bookCopy = _bookCopyServices.GetBookCopyDetails(id);

            if (bookCopy is null)
            {
                return NotFound();
            }

            var viewModel = new AdminBookCopyEditViewModel
            {
                Id = bookCopy.Id,
                InventoryNumber = bookCopy.InventoryNumber,
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(AdminBookCopyEditViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var dto = new BookCopyEditDto
            {
                Id = viewModel.Id,
                InventoryNumber = viewModel.InventoryNumber,
            };

            var result = _bookCopyServices.AdminUpdateBookCopy(dto);

            switch (result)
            {
                case BookCopyUpdateResult.Success:
                    return RedirectToAction(
                        nameof(Details),
                        new { bookCopyId = viewModel.Id });

                case BookCopyUpdateResult.NotFound:
                    return NotFound();


                case BookCopyUpdateResult.InventoryNumberAlreadyExists:
                    ModelState.AddModelError(
                        nameof(viewModel.InventoryNumber),
                        "این شماره اموال قبلاً برای نسخه دیگری ثبت شده است.");

                    return View(viewModel);


                default:
                    throw new InvalidOperationException(
                        $"Unexpected bookCopy update result: {result}");
            }
        }

        #endregion

        #region Create

        [HttpGet]
        public IActionResult Create(int bookId)
        {
            if (bookId <= 0)
                return NotFound();

            var book = _bookService.GetById(bookId);

            if (book is null)
                return NotFound();

            var viewModel = new BookCopyCreateViewModel
            {
                BookId = book.Id,
                Title = book.Title,
                Author = book.Author
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BookCopyCreateViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var dto = new BookCopyCreateDto
            {
                BookId = viewModel.BookId,
                InventoryNumber = viewModel.InventoryNumber,
            };

            try
            {
                int bookCopyId = _bookCopyServices.AdminCreateBookCopy(dto);

                return RedirectToAction(nameof(Details), new { bookCopyId });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(
                    nameof(viewModel.InventoryNumber),
                    ErrorMessageHelper.Translate(ex.Message));

                return View(viewModel);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        #endregion
    }
}
