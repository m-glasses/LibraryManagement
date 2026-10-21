using LibraryManagement.Areas.Admin.ViewModels.Book;
using LibraryManagement.DTO.Book;
using LibraryManagement.DTO.Book.Admin;
using LibraryManagement.Helper;
using LibraryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Areas.Admin.Controllers
{
    [Area(Roles.Admin)]
    [Authorize(Roles = Roles.Admin)]
    public class BookController : Controller
    {
        #region Fields and Constructor

        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        #endregion

        #region Index

        public IActionResult Index(AdminBookFilterViewModel inputViewModel)
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
                "PublicationYear",
                "TotalCopies",
                "AvailableCopies"
            ];

            if (!string.IsNullOrWhiteSpace(inputViewModel.SortBy) &&
                !allowedSortFields.Contains(
                    inputViewModel.SortBy,
                    StringComparer.OrdinalIgnoreCase))
            {
                inputViewModel.SortBy = null;
                ModelState.Remove(nameof(inputViewModel.SortBy));
            }

            var queryDto = new BookFilterDto
            {
                SearchTerm = inputViewModel.SearchTerm?.Trim(),
                HasAvailableCopies = inputViewModel.HasAvailableCopies,
                HasActiveReservations = inputViewModel.HasActiveReservations,
                SortBy = inputViewModel.SortBy,
                SortDescending = inputViewModel.SortDescending,
                Page = inputViewModel.Page,
                PageSize = inputViewModel.PageSize
            };

            try
            {
                var result = _bookService.GetBookList(queryDto);

                var viewModel = new AdminBookIndexViewModel
                {
                    Books = result.Books.Select(book => new AdminBookListViewModel
                    {
                        Id = book.Id,
                        Title = book.Title,
                        Author = book.Author,
                        Publisher = book.Publisher,
                        PublicationYear = book.PublicationYear,
                        TotalCopies = book.TotalCopies,
                        AvailableCopies = book.AvailableCopies,
                        ActiveReservations = book.ActiveReservations
                    }).ToList(),

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

            return View(new AdminBookIndexViewModel
            {
                Books = [],
                Filter = inputViewModel,
                TotalCount = 0,
                TotalPages = 0
            });
        }

        #endregion

        #region Details

        public IActionResult Details(int bookId)
        {
            if (bookId <= 0)
            {
                return NotFound();
            }

            var dto = _bookService.AdminGetBookDetails(bookId);

            if (dto is null)
            {
                return NotFound();
            }

            var viewModel = new AdminBookDetailsViewModel
            {
                Id = dto.Id,
                Title = dto.Title,
                Author = dto.Author,
                Publisher = dto.Publisher,
                PublicationYear = dto.PublicationYear,
                PublicationSeason = dto.PublicationSeason,
                PageCount = dto.PageCount,
                Edition = dto.Edition,
                Volume = dto.Volume,
                TotalCopies = dto.TotalCopies,
                AvailableCopies = dto.AvailableCopies,
                ActiveReservations = dto.ActiveReservations,
                ActiveLoans = dto.ActiveLoans
            };

            return View(viewModel);
        }

        #endregion

        #region Create

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AdminBookCreateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var dto = new AdminBookCreateDto
            {
                Title = viewModel.Title,
                Author = viewModel.Author,
                Publisher = viewModel.Publisher,
                PublicationYear = viewModel.PublicationYear,
                PublicationSeason = viewModel.PublicationSeason,
                PageCount = viewModel.PageCount,
                Edition = viewModel.Edition,
                Volume = viewModel.Volume
            };

            int bookId = _bookService.AdminCreateBook(dto);

            return RedirectToAction(nameof(Details), new { bookId });
        }

        #endregion

        #region Edit

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var book = _bookService.AdminGetBookDetails(id);

            if (book is null)
            {
                return NotFound();
            }

            var viewModel = new AdminBookEditViewModel
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Publisher = book.Publisher,
                PublicationYear = book.PublicationYear,
                PublicationSeason = book.PublicationSeason,
                PageCount = book.PageCount,
                Edition = book.Edition,
                Volume = book.Volume
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(AdminBookEditViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var dto = new AdminBookEditDto
            {
                Id = viewModel.Id,
                Title = viewModel.Title,
                Author = viewModel.Author,
                Publisher = viewModel.Publisher,
                PublicationYear = viewModel.PublicationYear,
                PublicationSeason = viewModel.PublicationSeason,
                PageCount = viewModel.PageCount,
                Edition = viewModel.Edition,
                Volume = viewModel.Volume
            };

            var result = _bookService.AdminUpdateBook(dto);

            switch (result)
            {
                case BookUpdateResult.Success:
                    return RedirectToAction(
                        nameof(Details),
                        new { bookId = viewModel.Id });

                case BookUpdateResult.NotFound:
                    return NotFound();

                default:
                    throw new InvalidOperationException(
                        $"Unexpected book update result: {result}");
            }
        }

        #endregion
    }
}
