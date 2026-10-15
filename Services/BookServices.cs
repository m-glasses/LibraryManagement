using LibraryManagement.Data;
using LibraryManagement.DTO.Book;
using LibraryManagement.DTO.Book.Admin;
using LibraryManagement.Models;
using LibraryManagement.Services.Interfaces;
using LibraryManagement.ViewModels.Book;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Services
{
    public class BookService : GenericService<Book>, IBookService
    {
        private readonly LibraryDbContext _context;
        public BookService(LibraryDbContext context) : base(context)
        {
            _context = context;
        }

        public BookDetailsViewModel GetBookDetails(int id)
        {
            return _context.Books
                .Where(b => b.Id == id)
                .Select(book => new BookDetailsViewModel
                {
                    Id = book.Id,
                    Title = book.Title,
                    Author = book.Author,
                    Publisher = book.Publisher,
                    PublicationYear = book.PublicationYear,
                    PublicationSeason = book.PublicationSeason,
                    PageCount = book.PageCount,
                    Edition = book.Edition,
                    Volume = book.Volume,
                    TotalCopies = book.BookCopies.Count(),
                    AvailableCopies = book.BookCopies.Count(
                        bc => !bc.Loans.Any(
                            l => l.LoanStatus == LoanStatus.Active || l.LoanStatus == LoanStatus.ReturnPending))
                })
                .SingleOrDefault()!;
        }


        public List<BookListViewModel> GetBookList(BookListQueryDto input)
        {
            ArgumentNullException.ThrowIfNull(input);

            if (input.Page < 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(input.Page),
                    "Page must be greater than zero.");
            }

            if (input.PageSize < 1 || input.PageSize > 50)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(input.PageSize),
                    "PageSize must be between 1 and 50.");
            }

            IQueryable<Book> booksQuery = _context.Books.AsNoTracking();

            booksQuery = ApplyBookFilters(booksQuery, filter);
            booksQuery = ApplyBookSorting(booksQuery, filter.SortBy, filter.SortDescending);

            return booksQuery
                .Select(book => new BookListViewModel
                {
                    Id = book.Id,
                    Title = book.Title,
                    Author = book.Author,
                    PublicationYear = book.PublicationYear,
                    Publisher = book.Publisher,

                    TotalCopies = book.BookCopies.Count(),

                    AvailableCopies = book.BookCopies.Count(bookCopy =>
                        !bookCopy.Loans.Any(loan =>
                            loan.LoanStatus != LoanStatus.Completed))
                })
                .ToList();
        }




        public BookListResult GetAllBooks(BookListQueryDto input)
        {
            ArgumentNullException.ThrowIfNull(input);

            if (input.Page < 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(input.Page),
                    "Page must be greater than zero.");
            }

            if (input.PageSize < 1 || input.PageSize > 50)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(input.PageSize),
                    "PageSize must be between 1 and 50.");
            }

            IQueryable<Book> booksQuery = _context.Books.AsNoTracking();

            // 1. Apply shared filters
            booksQuery = ApplyBookFilters(booksQuery, input);

            // 2. Count filtered results before pagination
            var totalCount = booksQuery.Count();

            var skip = (int)Math.Min(
                ((long)input.Page - 1) * input.PageSize,
                int.MaxValue);

            // 3. Sorting
           booksQuery = ApplyBookSorting(booksQuery, input.SortBy , input.SortDescending);

            // 4. Pagination and projection
            var books = booksQuery
                .Skip(skip)
                .Take(input.PageSize)
                .Select(book => new BookListDto
                {
                    Id = book.Id,
                    Title = book.Title,
                    Author = book.Author,
                    Publisher = book.Publisher,
                    PublicationYear = book.PublicationYear,

                    TotalCopies = book.BookCopies.Count(),

                    AvailableCopies = book.BookCopies.Count(bookCopy =>
                        !bookCopy.Loans.Any(loan =>
                            loan.LoanStatus != LoanStatus.Completed)),

                    ActiveReservations = book.Reservations.Count(reservation =>
                        reservation.ReservationStatus == ReservationStatus.Active)
                })
                .ToList();

            return new BookListResult
            {
                Books = books,
                CurrentPage = input.Page,
                PageSize = input.PageSize,
                TotalCount = totalCount
            };
        }

        public AdminBookDetailsDto? AdminGetBookDetails(int id)
        {
            if (id <= 0)
                return null;

            var book = _context.Books
                .AsNoTracking()
                .Where(book => book.Id == id)
                .Select(book => new AdminBookDetailsDto
                {
                    Id = book.Id,
                    Title = book.Title,
                    Author = book.Author,
                    Publisher = book.Publisher,
                    PublicationSeason = book.PublicationSeason,
                    PublicationYear = book.PublicationYear,
                    PageCount = book.PageCount,
                    Edition = book.Edition,
                    Volume = book.Volume,

                    TotalCopies = book.BookCopies.Count(),

                    AvailableCopies = book.BookCopies.Count(bookCopy =>
                        !bookCopy.Loans.Any(loan =>
                            loan.LoanStatus != LoanStatus.Completed)),

                    ActiveReservations = book.Reservations.Count(reservation =>
                        reservation.ReservationStatus == ReservationStatus.Active)
                })
                .FirstOrDefault();

            return book;
        }


        private IQueryable<Book> ApplyBookFilters(IQueryable<Book> booksQuery,BookFilterDto filter)
        {
            ArgumentNullException.ThrowIfNull(booksQuery);
            ArgumentNullException.ThrowIfNull(filter);

            // 1. Search
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var searchTerm = filter.SearchTerm.Trim();

                booksQuery = booksQuery.Where(book =>
                    book.Title.Contains(searchTerm) ||
                    book.Author.Contains(searchTerm) ||
                    book.Publisher.Contains(searchTerm));
            }

            // 2. Filter by available copies
            if (filter.HasAvailableCopies.HasValue)
            {
                booksQuery = booksQuery.Where(book =>
                    book.BookCopies.Any(bookCopy =>
                        !bookCopy.Loans.Any(loan =>
                            loan.LoanStatus != LoanStatus.Completed))
                    == filter.HasAvailableCopies.Value);
            }

            // 3. Filter by active reservations
            if (filter.HasActiveReservations.HasValue)
            {
                booksQuery = booksQuery.Where(book =>
                    book.Reservations.Any(reservation =>
                        reservation.ReservationStatus == ReservationStatus.Active)
                    == filter.HasActiveReservations.Value);
            }

            return booksQuery;
        }

        private IQueryable<Book> ApplyBookSorting(IQueryable<Book> booksQuery,string? sortBy,bool sortDescending)
        {
            ArgumentNullException.ThrowIfNull(booksQuery);

            return sortBy switch
            {
                "Title" => sortDescending
                    ? booksQuery.OrderByDescending(book => book.Title)
                        .ThenBy(book => book.Id)
                    : booksQuery.OrderBy(book => book.Title)
                        .ThenBy(book => book.Id),

                "Author" => sortDescending
                    ? booksQuery.OrderByDescending(book => book.Author)
                        .ThenBy(book => book.Id)
                    : booksQuery.OrderBy(book => book.Author)
                        .ThenBy(book => book.Id),

                "PublicationYear" => sortDescending
                    ? booksQuery.OrderByDescending(book => book.PublicationYear)
                        .ThenBy(book => book.Id)
                    : booksQuery.OrderBy(book => book.PublicationYear)
                        .ThenBy(book => book.Id),

                "TotalCopies" => sortDescending
                    ? booksQuery.OrderByDescending(book => book.BookCopies.Count())
                        .ThenBy(book => book.Id)
                    : booksQuery.OrderBy(book => book.BookCopies.Count())
                        .ThenBy(book => book.Id),

                "AvailableCopies" => sortDescending
                    ? booksQuery.OrderByDescending(book =>
                        book.BookCopies.Count(bookCopy =>
                            !bookCopy.Loans.Any(loan =>
                                loan.LoanStatus != LoanStatus.Completed)))
                        .ThenBy(book => book.Id)
                    : booksQuery.OrderBy(book =>
                        book.BookCopies.Count(bookCopy =>
                            !bookCopy.Loans.Any(loan =>
                                loan.LoanStatus != LoanStatus.Completed)))
                        .ThenBy(book => book.Id),

                _ => booksQuery.OrderBy(book => book.Id)
            };
        }


    }
}
