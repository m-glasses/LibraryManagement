using LibraryManagement.Data;
using LibraryManagement.DTO.Book;
using LibraryManagement.DTO.Book.Admin;
using LibraryManagement.DTO.Book.User;
using LibraryManagement.Models;
using LibraryManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Services
{
    public class BookService : GenericService<Book>, IBookService
    {
        private readonly LibraryDbContext _context;

        public BookService(LibraryDbContext context)
            : base(context)
        {
            _context = context;
        }

        public BookListResult GetBookList(BookListQueryDto input)
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

            IQueryable<Book> booksQuery = _context.Books
                .AsNoTracking();

            // 1. Apply shared filters.
            booksQuery = ApplyBookFilters(booksQuery, input);

            // 2. Count filtered books before pagination.
            var totalCount = booksQuery.Count();

            // 3. Apply deterministic sorting.
            booksQuery = ApplyBookSorting(booksQuery ,input.SortBy ,input.SortDescending);

            // 4. Apply pagination and project to DTO.
            var skip = (int)Math.Min(((long)input.Page - 1) * input.PageSize, int.MaxValue);

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

                    AvailableCopies = book.BookCopies.Count(
                        bookCopy => !bookCopy.Loans.Any(
                            loan => loan.LoanStatus == LoanStatus.Active ||
                                    loan.LoanStatus == LoanStatus.ReturnPending)),

                    ActiveReservations = book.Reservations.Count(
                        reservation =>
                            reservation.ReservationStatus ==
                            ReservationStatus.Active)
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

        public UserBookDetailDto? GetBookDetails(int id)
        {
            if (id <= 0)
            {
                return null;
            }

            return _context.Books
                .AsNoTracking()
                .Where(book => book.Id == id)
                .Select(book => new UserBookDetailDto
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
                        bookCopy => !bookCopy.Loans.Any(
                            loan => loan.LoanStatus == LoanStatus.Active ||
                                    loan.LoanStatus == LoanStatus.ReturnPending)),
                    ActiveReservations = book.Reservations.Count(reservation => reservation.ReservationStatus == ReservationStatus.Active)

                    
                })
                .FirstOrDefault();
        }

        public AdminBookDetailsDto? AdminGetBookDetails(int id)
        {
            if (id <= 0)
            {
                return null;
            }

            return _context.Books
                .AsNoTracking()
                .Where(book => book.Id == id)
                .Select(book => new AdminBookDetailsDto
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
                        bookCopy => !bookCopy.Loans.Any(
                            loan => loan.LoanStatus == LoanStatus.Active ||
                                    loan.LoanStatus == LoanStatus.ReturnPending)),

                    ActiveReservations = book.Reservations.Count(
                        reservation =>
                            reservation.ReservationStatus ==
                            ReservationStatus.Active),

                    ActiveLoans = book.BookCopies
                        .SelectMany(bookCopy => bookCopy.Loans)
                        .Count(loan =>
                            loan.LoanStatus == LoanStatus.Active ||
                            loan.LoanStatus == LoanStatus.ReturnPending)
                })
                .FirstOrDefault();
        }

        public BookUpdateResult AdminUpdateBook(AdminBookEditDto input)
        {
            ArgumentNullException.ThrowIfNull(input);

            var book = _context.Books
                .FirstOrDefault(book => book.Id == input.Id);

            if (book is null)
            {
                return BookUpdateResult.NotFound;
            }

            book.Title = input.Title;
            book.Author = input.Author;
            book.Publisher = input.Publisher;
            book.PublicationYear = input.PublicationYear;
            book.PublicationSeason = input.PublicationSeason;
            book.PageCount = input.PageCount;
            book.Edition = input.Edition;
            book.Volume = input.Volume;

            _context.SaveChanges();

            return BookUpdateResult.Success;
        }


        public int AdminCreateBook(AdminBookCreateDto input)
        {
            ArgumentNullException.ThrowIfNull(input);

            var book = new Book
            {
                Title = input.Title,
                Author = input.Author,
                Publisher = input.Publisher,
                PublicationYear = input.PublicationYear,
                PublicationSeason = input.PublicationSeason,
                PageCount = input.PageCount,
                Edition = input.Edition,
                Volume = input.Volume
            };

            _context.Books.Add(book);
            _context.SaveChanges();

            return book.Id;
        }


        private IQueryable<Book> ApplyBookFilters(
            IQueryable<Book> booksQuery,
            BookFilterDto filter)
        {
            ArgumentNullException.ThrowIfNull(booksQuery);
            ArgumentNullException.ThrowIfNull(filter);

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var searchTerm = filter.SearchTerm.Trim();

                booksQuery = booksQuery.Where(book =>
                    book.Title.Contains(searchTerm) ||
                    book.Author.Contains(searchTerm) ||
                    book.Publisher.Contains(searchTerm));
            }

            if (filter.HasAvailableCopies.HasValue)
            {
                booksQuery = booksQuery.Where(book =>
                    book.BookCopies.Any(
                        bookCopy => !bookCopy.Loans.Any(
                            loan =>
                                loan.LoanStatus == LoanStatus.Active ||
                                loan.LoanStatus == LoanStatus.ReturnPending))
                    == filter.HasAvailableCopies.Value);
            }

            if (filter.HasActiveReservations.HasValue)
            {
                booksQuery = booksQuery.Where(book =>
                    book.Reservations.Any(
                        reservation =>
                            reservation.ReservationStatus ==
                            ReservationStatus.Active)
                    == filter.HasActiveReservations.Value);
            }

            return booksQuery;
        }

        private IQueryable<Book> ApplyBookSorting(
            IQueryable<Book> booksQuery,
            string? sortBy,
            bool sortDescending)
        {
            ArgumentNullException.ThrowIfNull(booksQuery);

            // Normalize the sort key so casing doesn't affect the result.
            var normalizedSortBy = sortBy?.Trim().ToUpperInvariant();

            return normalizedSortBy switch
            {
                "TITLE" => sortDescending
                    ? booksQuery.OrderByDescending(book => book.Title)
                        .ThenBy(book => book.Id)
                    : booksQuery.OrderBy(book => book.Title)
                        .ThenBy(book => book.Id),

                "AUTHOR" => sortDescending
                    ? booksQuery.OrderByDescending(book => book.Author)
                        .ThenBy(book => book.Id)
                    : booksQuery.OrderBy(book => book.Author)
                        .ThenBy(book => book.Id),

                "PUBLICATIONYEAR" => sortDescending
                    ? booksQuery.OrderByDescending(
                            book => book.PublicationYear)
                        .ThenBy(book => book.Id)
                    : booksQuery.OrderBy(
                            book => book.PublicationYear)
                        .ThenBy(book => book.Id),

                "TOTALCOPIES" => sortDescending
                    ? booksQuery.OrderByDescending(
                            book => book.BookCopies.Count())
                        .ThenBy(book => book.Id)
                    : booksQuery.OrderBy(
                            book => book.BookCopies.Count())
                        .ThenBy(book => book.Id),

                "AVAILABLECOPIES" => sortDescending
                    ? booksQuery.OrderByDescending(
                            book => book.BookCopies.Count(
                                bookCopy => !bookCopy.Loans.Any(
                                    loan =>
                                        loan.LoanStatus == LoanStatus.Active ||
                                        loan.LoanStatus == LoanStatus.ReturnPending)))
                        .ThenBy(book => book.Id)
                    : booksQuery.OrderBy(
                            book => book.BookCopies.Count(
                                bookCopy => !bookCopy.Loans.Any(
                                    loan =>
                                        loan.LoanStatus == LoanStatus.Active ||
                                        loan.LoanStatus == LoanStatus.ReturnPending)))
                        .ThenBy(book => book.Id),

                _ => booksQuery.OrderBy(book => book.Id)
            };
        }
    }
}