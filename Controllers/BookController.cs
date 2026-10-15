using LibraryManagement.DTO.Book;
using LibraryManagement.Helper;
using LibraryManagement.Models;
using LibraryManagement.Services.Interfaces;
using LibraryManagement.ViewModels.Book;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookService _bookService;
        private readonly ILoanService _loanService;
        private readonly UserManager<User> _userManager;
        public BookController(IBookService bookService, ILoanService loanService, UserManager<User> userManager)
        {
            _bookService = bookService;
            _loanService = loanService;
            _userManager = userManager;
        }


        // GET: BookController
        public IActionResult Index(BookFilterViewModel inputViewModel)
        {
            ArgumentNullException.ThrowIfNull(inputViewModel);

            // Validate pagination values
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

            // Allow only supported sorting fields
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

            var queryDto = new BookListQueryDto
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

                var viewModel = new BookIndexViewModel
                {
                    Books = result.Books.Select(book => new BookListViewModel
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

                var viewModel = new BookIndexViewModel
                {
                    Books = new List<BookListViewModel>(),
                    Filter = inputViewModel,
                    TotalCount = 0,
                    TotalPages = 0
                };

                return View(viewModel);
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] =
                    ErrorMessageHelper.Translate(ex.Message);

                var viewModel = new BookIndexViewModel
                {
                    Books = new List<BookListViewModel>(),
                    Filter = inputViewModel,
                    TotalCount = 0,
                    TotalPages = 0
                };

                return View(viewModel);
            }            
        }

        // GET: BookController/Details/5
        public IActionResult Details(int bookId)
        {
            if(bookId <= 0)
            {
                return NotFound();
            }

            var dto = _bookService.GetBookDetails(bookId);
            if (dto == null)
            {
                return NotFound();
            }
            var viewModel = new BookDetailsViewModel()
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
            };

            return View(viewModel);
        }

        // GET: BookController/Create
        public IActionResult Create()
        {
            return View();
        }




        // POST: BookController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Book book)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _bookService.Create(book);
                    return RedirectToAction(nameof(Index));
                }
                return View(book);
            }
            catch
            {
                return View();
            }
        }

        // GET: BookController/Edit/5
        public IActionResult Edit(int id)
        {
            var book = _bookService.GetById(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // POST: BookController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Book book)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isValid = _bookService.Update(book);
                    if (!isValid)
                    {
                        return View(book);
                    }
                    return RedirectToAction(nameof(Index));
                }
                return View(book);
            }
            catch
            {
                return View();
            }
        }

        // GET: BookController/Delete/5
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var book = _bookService.GetById(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // POST: BookController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                bool isValid = _bookService.Delete(id);
                if (!isValid)
                {
                    var book = _bookService.GetById(id);
                    return View(book);
                }
                return RedirectToAction(nameof(Index));

            }
            catch
            {
                return View();
            }
        }
    }
}

