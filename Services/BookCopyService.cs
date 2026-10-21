using LibraryManagement.Areas.Admin.ViewModels.BookCopies;
using LibraryManagement.Data;
using LibraryManagement.DTO.Book.Admin;
using LibraryManagement.DTO.BookCopies;
using LibraryManagement.Models;
using LibraryManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Services
{
    public class BookCopyService : GenericService<BookCopy>, IBookCopyService
    {
        private readonly LibraryDbContext _context;
        public BookCopyService(LibraryDbContext context) : base(context)
        {
            _context = context;
        }

        public List<BookCopy> GetAvailableCopies()
        {
            var bookCopies = _context.BookCopies
                .Where(bc => !bc.Loans.Any(loan =>
                    loan.LoanStatus != LoanStatus.Completed))
                    .ToList();
            return bookCopies;

        }

        public BookCopiesListResult GetBookCopiesList(BookCopiesFilterDto input)
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

            IQueryable<BookCopy> bookCopiesQuery = _context.BookCopies
                .AsNoTracking();

            // 1. Apply filters.
            bookCopiesQuery = ApplyBookCopyFilters(bookCopiesQuery, input);

            // 2. Count filtered results before pagination.
            var totalCount = bookCopiesQuery.Count();

            // 3. Apply sorting.
            bookCopiesQuery = ApplyBookCopySorting(
                bookCopiesQuery,
                input.SortBy,
                input.SortDescending);

            // 4. Calculate pagination safely.
            var skip = (input.Page - 1) * input.PageSize;

            // 5. Select the latest loan and project to DTO.
            var bookCopies = bookCopiesQuery
                .Skip(skip)
                .Take(input.PageSize)
                .Select(bookCopy => new
                {
                    BookCopy = bookCopy,

                    LastLoan = bookCopy.Loans
                        .OrderByDescending(loan => loan.StartDate)
                        .ThenByDescending(loan => loan.Id)
                        .Select(loan => new
                        {
                            loan.StartDate,
                            loan.DueDate,
                            BorrowerName = loan.User.Name
                        })
                        .FirstOrDefault()
                })
                .Select(item => new BookCopiesListDto
                {
                    Id = item.BookCopy.Id,
                    BookID = item.BookCopy.BookId,
                    Title = item.BookCopy.Book.Title,
                    Author = item.BookCopy.Book.Author,
                    Publisher = item.BookCopy.Book.Publisher,
                    InventoryNumber = item.BookCopy.InventoryNumber,

                    Status = item.BookCopy.Loans.Any(loan =>
                        loan.LoanStatus == LoanStatus.ReturnPending)
                            ? BookCopyStatus.ReturnPending
                            : item.BookCopy.Loans.Any(loan =>
                                loan.LoanStatus == LoanStatus.Active)
                                    ? BookCopyStatus.Borrowed
                                    : BookCopyStatus.Available,

                    LastLoanStartDate = item.LastLoan == null
                        ? null
                        : item.LastLoan.StartDate,

                    LastLoanDueDate = item.LastLoan == null
                        ? null
                        : item.LastLoan.DueDate,

                    LastBorrowerName = item.LastLoan == null
                        ? null
                        : item.LastLoan.BorrowerName
                })
                .ToList();

            return new BookCopiesListResult
            {
                BookCopies = bookCopies,
                CurrentPage = input.Page,
                PageSize = input.PageSize,
                TotalCount = totalCount
            };
        }

        public AdminBookCopyDetailDto? GetBookCopyDetails(int id)
        {
            if (id <= 0)
            {
                return null;
            }

            return _context.BookCopies
                .AsNoTracking()
                .Where(bookCopy => bookCopy.Id == id)
                .Select(bookCopy => new
                {
                    BookCopy = bookCopy,
                    LastLoan = bookCopy.Loans
                        .OrderByDescending(loan => loan.StartDate)
                        .Select(loan => new
                        {
                            loan.StartDate,
                            loan.DueDate,
                            BorrowerName = loan.User.Name
                        }).FirstOrDefault()
                })
                .Select(item => new AdminBookCopyDetailDto
                {
                    Id = item.BookCopy.Id,
                    BookId = item.BookCopy.BookId,
                    Title = item.BookCopy.Book.Title,
                    Author = item.BookCopy.Book.Author,
                    Publisher = item.BookCopy.Book.Publisher,
                    PublicationYear = item.BookCopy.Book.PublicationYear,
                    PublicationSeason = item.BookCopy.Book.PublicationSeason,
                    PageCount = item.BookCopy.Book.PageCount,
                    Edition = item.BookCopy.Book.Edition,
                    Volume = item.BookCopy.Book.Volume,
                    InventoryNumber = item.BookCopy.InventoryNumber,

                    Status = item.BookCopy.Loans.Any(loan =>
                        loan.LoanStatus == LoanStatus.ReturnPending)
                            ? BookCopyStatus.ReturnPending
                            : item.BookCopy.Loans.Any(loan =>
                                loan.LoanStatus == LoanStatus.Active)
                                    ? BookCopyStatus.Borrowed
                                    : BookCopyStatus.Available,

                    LastLoanStartDate = item.LastLoan == null
                        ? null
                        : item.LastLoan.StartDate,

                    LastLoanDueDate = item.LastLoan == null
                        ? null
                        : item.LastLoan.DueDate,

                    LastBorrowerName = item.LastLoan == null
                        ? null
                        : item.LastLoan.BorrowerName


                }).FirstOrDefault();
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


        private IQueryable<BookCopy> ApplyBookCopyFilters(IQueryable<BookCopy> bookCopiesQuery,BookCopiesFilterDto filter)
        {
            ArgumentNullException.ThrowIfNull(bookCopiesQuery);
            ArgumentNullException.ThrowIfNull(filter);

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var searchTerm = filter.SearchTerm.Trim();

                bookCopiesQuery = bookCopiesQuery.Where(bookCopy =>
                    bookCopy.Book.Title.Contains(searchTerm) ||
                    bookCopy.Book.Author.Contains(searchTerm) ||
                    bookCopy.Book.Publisher.Contains(searchTerm)||
                    bookCopy.InventoryNumber.Contains(searchTerm));
            }

            bookCopiesQuery = filter.Status switch
            {
                BookCopyStatus.Available => bookCopiesQuery.Where(bookCopy =>
                !bookCopy.Loans.Any(loan =>
                                loan.LoanStatus == LoanStatus.Active ||
                                loan.LoanStatus == LoanStatus.ReturnPending)),

                BookCopyStatus.Borrowed => bookCopiesQuery.Where(bookCopy =>
                bookCopy.Loans.Any(loan =>
                                loan.LoanStatus == LoanStatus.Active)),

                BookCopyStatus.ReturnPending => bookCopiesQuery.Where(bookCopy =>
                bookCopy.Loans.Any(loan =>
                                loan.LoanStatus == LoanStatus.ReturnPending)),

                _ => bookCopiesQuery

            }; 

            return bookCopiesQuery;
        }

        private IQueryable<BookCopy> ApplyBookCopySorting(IQueryable<BookCopy> bookCopiesQuery, string? sortBy,bool sortDescending)
        {
            ArgumentNullException.ThrowIfNull(bookCopiesQuery);

            // Normalize the sort key so casing doesn't affect the result.
            var normalizedSortBy = sortBy?.Trim().ToUpperInvariant();

            return normalizedSortBy switch
            {
                "TITLE" => sortDescending
                    ? bookCopiesQuery.OrderByDescending(bookCopy => bookCopy.Book.Title)
                        .ThenBy(bookCopy => bookCopy.Id)
                    : bookCopiesQuery.OrderBy(bookCopy => bookCopy.Book.Title)
                        .ThenBy(bookCopy => bookCopy.Id),

                "AUTHOR" => sortDescending
                    ? bookCopiesQuery.OrderByDescending(bookCopy => bookCopy.Book.Author)
                        .ThenBy(bookCopy => bookCopy.Id)
                    : bookCopiesQuery.OrderBy(bookCopy => bookCopy.Book.Author)
                        .ThenBy(bookCopy => bookCopy.Id),

                "PUBLISHER" => sortDescending
                    ? bookCopiesQuery.OrderByDescending(
                            bookCopy => bookCopy.Book.Publisher)
                        .ThenBy(bookCopy => bookCopy.Id)
                    : bookCopiesQuery.OrderBy(
                             bookCopy => bookCopy.Book.Publisher)
                        .ThenBy(bookCopy => bookCopy.Id),

                "INVENTORYNUMBER" => sortDescending
                    ? bookCopiesQuery.OrderByDescending(
                            bookCopy => bookCopy.InventoryNumber)
                        .ThenBy(bookCopy => bookCopy.Id)
                    : bookCopiesQuery.OrderBy(
                            bookCopy => bookCopy.InventoryNumber)
                        .ThenBy(bookCopy => bookCopy.Id),

                "AVAILABLE" => sortDescending
                    ? bookCopiesQuery.OrderByDescending(
                                bookCopy => !bookCopy.Loans.Any(
                                    loan =>
                                        loan.LoanStatus == LoanStatus.Active ||
                                        loan.LoanStatus == LoanStatus.ReturnPending))
                        .ThenBy(bookCopy => bookCopy.Id)
                    : bookCopiesQuery.OrderBy(
                                bookCopy => !bookCopy.Loans.Any(
                                    loan =>
                                        loan.LoanStatus == LoanStatus.Active ||
                                        loan.LoanStatus == LoanStatus.ReturnPending))
                        .ThenBy(bookCopy => bookCopy.Id),

                _ => bookCopiesQuery.OrderBy(bookCopy => bookCopy.Id)
            };
        }
    }
}

