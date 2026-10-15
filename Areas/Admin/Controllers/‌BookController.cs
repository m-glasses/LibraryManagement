using LibraryManagement.Areas.Admin.ViewModels;
using LibraryManagement.DTO;
using LibraryManagement.DTO.Book;
using LibraryManagement.Helper;
using LibraryManagement.Models;
using LibraryManagement.Services;
using LibraryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LibraryManagement.ViewModels.Book;

namespace LibraryManagement.Areas.Admin.Controllers
{
    [Area(Roles.Admin)]
    [Authorize(Roles = Roles.Admin)]
    public class BookController : Controller
    {
        private readonly IBookService _bookService;
        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }


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
                var result = _bookService.GetAllBooks(queryDto);

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



        public IActionResult Details(int bookId)
        {
            if (bookId <= 0)
                return NotFound();

            var dto = _bookService.AdminGetBookDetails(bookId);

            if (dto is null)
                return NotFound();

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
                ActiveReservations = dto.ActiveReservations
            };

            return View(viewModel);
        }
    }
    
   
}

