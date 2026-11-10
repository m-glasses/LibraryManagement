using LibraryManagement.Data;
using LibraryManagement.DTO.Book;
using LibraryManagement.DTO.Loans;
using LibraryManagement.Models;
using LibraryManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Services
{
    public class LoanService : GenericService<Loan>, ILoanService
    {
        private readonly LibraryDbContext _context;
        private readonly IWalletService _walletService;
        public LoanService(LibraryDbContext context, IWalletService wallet) : base(context)
        {
            _context = context;
            _walletService = wallet;
        }

        public BookCopy GetAvailableBookCopy(int bookId)
        {
            var bookCopy = _context.BookCopies
                .FirstOrDefault(bookCopy =>
                    bookCopy.BookId == bookId &&
                    !bookCopy.Loans.Any(loan =>
                        loan.LoanStatus != LoanStatus.Completed));

            if (bookCopy is null)
            {
                throw new InvalidOperationException("Book not available");
            }

            return bookCopy;
        }

        private Reservation? ValidateReservationPriority(int userId, int bookId)
        {
            var firstActiveReservation = _context.Reservations
                .Where(reservation =>
                    reservation.BookId == bookId &&
                    reservation.ReservationStatus == ReservationStatus.Active)
                .OrderBy(reservation => reservation.ReservedAt)
                .FirstOrDefault();

            if (firstActiveReservation is not null &&
                firstActiveReservation.UserId != userId)
            {
                throw new InvalidOperationException(
                    "Another user has priority to borrow this book");
            }

            return firstActiveReservation;
        }


        public Loan Borrow(int userId, int bookId)
        {
            if (!_context.Users.Any(user => user.Id == userId))
            {
                throw new InvalidOperationException("User not found");
            }


            var wallet = _walletService.GetByUserId(userId);

            if (wallet is null)
            {
                throw new InvalidOperationException("Wallet not found.");
            }
            if (wallet.Balance < 0)
            {
                throw new InvalidOperationException("User cannot borrow while wallet balance is negative.");
            }

            if (!_context.Books.Any(book => book.Id == bookId))
            {
                throw new InvalidOperationException("Book not found");
            }

            var settings = _context.LibrarySettings.SingleOrDefault();

            if (settings is null)
            {
                throw new InvalidOperationException(
                    "Library settings not found");
            }

            var firstActiveReservation =
                ValidateReservationPriority(userId, bookId);

            var bookCopy = GetAvailableBookCopy(bookId);

            var startDate = DateTime.Now;

            var loan = new Loan
            {
                UserId = userId,
                BookCopyId = bookCopy.Id,
                StartDate = startDate,
                DueDate = startDate.AddDays(settings.LoanDurationDays),
                LoanStatus = LoanStatus.Active,
                RenewalCount = 0,
                LateFeePerDay = settings.LateFeePerDay,
                DailyRate = settings.DailyRate
            };

            _context.Loans.Add(loan);

            if (firstActiveReservation is not null)
            {
                firstActiveReservation.ReservationStatus =
                    ReservationStatus.Completed;
            }

            _context.SaveChanges();

            return loan;
        }


        public void RequestReturn(int loanId, int userId)
        {
            var loan = _context.Loans
                .FirstOrDefault(loan =>
                    loan.Id == loanId &&
                    loan.UserId == userId);

            if (loan is null)
            {
                throw new InvalidOperationException(
                    "Loan not found");
            }

            if (loan.LoanStatus != LoanStatus.Active)
            {
                throw new InvalidOperationException(
                    "The loan is not active");
            }

            loan.LoanStatus = LoanStatus.ReturnPending;

            _context.SaveChanges();
        }


        private void CalculateLoanAmount(Loan loan, DateTime returnDate)
        {
            var normalDays = Math.Min(
                (returnDate - loan.StartDate).Days,
                (loan.DueDate - loan.StartDate).Days);

            loan.CalculatedAmount =
                normalDays * loan.DailyRate;

            loan.LateDays =
                Math.Max(0, (returnDate - loan.DueDate).Days);

            loan.LateFee =
                loan.LateDays * loan.LateFeePerDay;

            loan.FinalAmount =
                loan.LateFee + loan.CalculatedAmount;
        }

        public void ConfirmReturn(int loanId, DateTime returnDate)
        {
            var loan = _context.Loans
                .FirstOrDefault(l => l.Id == loanId);

            if (loan is null)
            {
                throw new InvalidOperationException("Loan not found");
            }

            if (loan.LoanStatus is not LoanStatus.Active
                and not LoanStatus.ReturnPending)
            {
                throw new InvalidOperationException(
                    "The loan cannot be returned");
            }

            if (returnDate < loan.StartDate ||
                returnDate > DateTime.Now)
            {
                throw new InvalidOperationException(
                    "Invalid return date");
            }

            loan.ReturnDate = returnDate;

            CalculateLoanAmount(loan, returnDate);

            Wallet? wallet = _context.Wallets.FirstOrDefault(wallet => wallet.UserId == loan.UserId);

            if (wallet is null)
            {
                throw new InvalidOperationException("wallet not found");
            }

            _walletService.Withdraw(wallet.Id, loan.FinalAmount);

            loan.LoanStatus = LoanStatus.Completed;

            _context.SaveChanges();
        }

        public void Renew(int loanId, int userId)
        {
            var loan = _context.Loans
                .FirstOrDefault(loan =>
                    loan.Id == loanId &&
                    loan.UserId == userId);

            if (loan is null)
            {
                throw new InvalidOperationException(
                    "Loan not found");
            }

            if (loan.LoanStatus != LoanStatus.Active)
            {
                throw new InvalidOperationException(
                    "The loan is not active");
            }

            var settings = _context.LibrarySettings
                .SingleOrDefault();

            if (settings is null)
            {
                throw new InvalidOperationException(
                    "Library settings not found");
            }

            if (loan.RenewalCount >= settings.MaxRenewalCount)
            {
                throw new InvalidOperationException(
                    "Maximum renewal limit has been reached");
            }

            loan.DueDate = loan.DueDate
                .AddDays(settings.RenewalDurationDays);

            loan.RenewalCount++;

            _context.SaveChanges();
        }

        public Loan? GetDetailsById(int loanId, int userId)
        {
            return _context.Loans
                .Include(l => l.BookCopy)
                .ThenInclude(bc => bc.Book)
                .FirstOrDefault(l => l.Id == loanId && l.UserId == userId);
        }


        public UserLoanListResult UserGetLoansList(int userId, LoanFilterDto input)
        {
            ArgumentNullException.ThrowIfNull(input);

            if (userId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(userId));
            }

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

            IQueryable<Loan> loansQuery = _context.Loans
                .AsNoTracking()
                .Where(loan => loan.UserId == userId);

            // 1. Apply filters.
            loansQuery = ApplyLoanFilters(loansQuery, input, isAdmin: false);

            // 2. Count filtered loans before pagination.
            var totalCount = loansQuery.Count();

            // 3. Apply sorting.
            loansQuery = ApplyLoanSorting(
                loansQuery,
                input.SortBy,
                input.SortDescending,
                isAdmin: false);

            // 4. Calculate pagination.
            var skip = (int)Math.Min(
                ((long)input.Page - 1) * input.PageSize,
                int.MaxValue);

            // 5. Project to DTO and execute query.
            var loans = loansQuery
                .Skip(skip)
                .Take(input.PageSize)
                .Select(loan => new UserLoanListDto
                {
                    LoanId = loan.Id,
                    BookTitle = loan.BookCopy.Book.Title,
                    Author = loan.BookCopy.Book.Author,
                    InventoryNumber = loan.BookCopy.InventoryNumber,
                    StartDate = loan.StartDate,
                    DueDate = loan.DueDate,
                    ReturnDate = loan.ReturnDate,
                    Status = loan.LoanStatus,
                    FinalAmount = loan.FinalAmount
                })
                .ToList();

            return new UserLoanListResult
            {
                Loans = loans,
                CurrentPage = input.Page,
                PageSize = input.PageSize,
                TotalCount = totalCount
            };
        }


        public AdminLoanListResult AdminGetLoansList(LoanFilterDto input)
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

            IQueryable<Loan> loansQuery = _context.Loans
                .AsNoTracking();

            // 1. Apply filters.
            loansQuery = ApplyLoanFilters(loansQuery, input, isAdmin: true);

            // 2. Count filtered loans before pagination.
            var totalCount = loansQuery.Count();

            // 3. Apply sorting.
            loansQuery = ApplyLoanSorting(
                loansQuery,
                input.SortBy,
                input.SortDescending,
                isAdmin: true);

            // 4. Apply pagination and project to DTO.
            var skip = (int)Math.Min(
                ((long)input.Page - 1) * input.PageSize,
                int.MaxValue);

            var loans = loansQuery
                .Skip(skip)
                .Take(input.PageSize)
                .Select(loan => new AdminLoanListDto
                {
                    LoanId = loan.Id,
                    UserFullName = loan.User.Name + " " + loan.User.Family,
                    Email = loan.User.Email!,
                    BookTitle = loan.BookCopy.Book.Title,
                    Author = loan.BookCopy.Book.Author,
                    InventoryNumber = loan.BookCopy.InventoryNumber,
                    StartDate = loan.StartDate,
                    DueDate = loan.DueDate,
                    ReturnDate = loan.ReturnDate,
                    Status = loan.LoanStatus,
                    FinalAmount = loan.FinalAmount
                })
                .ToList();

            return new AdminLoanListResult
            {
                Loans = loans,
                CurrentPage = input.Page,
                PageSize = input.PageSize,
                TotalCount = totalCount
            };
        }



        public Loan? GetAdminDetailsById(int loanId)
        {
            return _context.Loans
                .AsNoTracking()
                .Include(loan => loan.User)
                .Include(loan => loan.BookCopy)
                    .ThenInclude(bookCopy => bookCopy.Book)
                .FirstOrDefault(loan => loan.Id == loanId);
        }



        private IQueryable<Loan> ApplyLoanFilters(
            IQueryable<Loan> loansQuery,
            LoanFilterDto filter,
            bool isAdmin)
        {
            ArgumentNullException.ThrowIfNull(loansQuery);
            ArgumentNullException.ThrowIfNull(filter);

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var searchTerm = filter.SearchTerm.Trim();

                loansQuery = loansQuery.Where(loan =>
                    loan.BookCopy.Book.Title.Contains(searchTerm) ||
                    loan.BookCopy.Book.Author.Contains(searchTerm) ||
                    loan.BookCopy.InventoryNumber.Contains(searchTerm) ||
                    (isAdmin &&
                        (loan.User.Name.Contains(searchTerm) ||
                         loan.User.Family.Contains(searchTerm) ||
                         loan.User.Email.Contains(searchTerm))));
            }

            if (filter.Status.HasValue)
            {
                loansQuery = loansQuery.Where(
                    loan => loan.LoanStatus == filter.Status.Value);
            }

            if (filter.StartDateFrom.HasValue)
            {
                loansQuery = loansQuery.Where(
                    loan => loan.StartDate >= filter.StartDateFrom.Value);
            }

            if (filter.StartDateTo.HasValue)
            {
                loansQuery = loansQuery.Where(
                    loan => loan.StartDate < filter.StartDateTo.Value.AddDays(1));
            }

            if (filter.DueDateFrom.HasValue)
            {
                loansQuery = loansQuery.Where(
                    loan => loan.DueDate >= filter.DueDateFrom.Value);
            }

            if (filter.DueDateTo.HasValue)
            {
                loansQuery = loansQuery.Where(
                    loan => loan.DueDate < filter.DueDateTo.Value.AddDays(1));
            }

            if (filter.ReturnDateFrom.HasValue)
            {
                loansQuery = loansQuery.Where(
                    loan => loan.ReturnDate >= filter.ReturnDateFrom.Value);
            }

            if (filter.ReturnDateTo.HasValue)
            {
                loansQuery = loansQuery.Where(
                    loan => loan.ReturnDate < filter.ReturnDateTo.Value.AddDays(1));
            }

            return loansQuery;
        }




        private IQueryable<Loan> ApplyLoanSorting(
            IQueryable<Loan> loansQuery,
            string? sortBy,
            bool sortDescending,
            bool isAdmin)
        {
            ArgumentNullException.ThrowIfNull(loansQuery);

            var normalizedSortBy = sortBy?.Trim().ToUpperInvariant();

            return normalizedSortBy switch
            {
                "TITLE" => sortDescending
                    ? loansQuery.OrderByDescending(loan => loan.BookCopy.Book.Title)
                        .ThenBy(loan => loan.Id)
                    : loansQuery.OrderBy(loan => loan.BookCopy.Book.Title)
                        .ThenBy(loan => loan.Id),

                "AUTHOR" => sortDescending
                    ? loansQuery.OrderByDescending(loan => loan.BookCopy.Book.Author)
                        .ThenBy(loan => loan.Id)
                    : loansQuery.OrderBy(loan => loan.BookCopy.Book.Author)
                        .ThenBy(loan => loan.Id),

                "INVENTORYNUMBER" => sortDescending
                    ? loansQuery.OrderByDescending(loan => loan.BookCopy.InventoryNumber)
                        .ThenBy(loan => loan.Id)
                    : loansQuery.OrderBy(loan => loan.BookCopy.InventoryNumber)
                        .ThenBy(loan => loan.Id),

                "STARTDATE" => sortDescending
                    ? loansQuery.OrderByDescending(loan => loan.StartDate)
                        .ThenBy(loan => loan.Id)
                    : loansQuery.OrderBy(loan => loan.StartDate)
                        .ThenBy(loan => loan.Id),

                "DUEDATE" => sortDescending
                    ? loansQuery.OrderByDescending(loan => loan.DueDate)
                        .ThenBy(loan => loan.Id)
                    : loansQuery.OrderBy(loan => loan.DueDate)
                        .ThenBy(loan => loan.Id),

                "RETURNDATE" => sortDescending
                    ? loansQuery.OrderByDescending(loan => loan.ReturnDate)
                        .ThenBy(loan => loan.Id)
                    : loansQuery.OrderBy(loan => loan.ReturnDate)
                        .ThenBy(loan => loan.Id),

                "NAME" when isAdmin => sortDescending
                    ? loansQuery.OrderByDescending(loan => loan.User.Name)
                        .ThenBy(loan => loan.Id)
                    : loansQuery.OrderBy(loan => loan.User.Name)
                        .ThenBy(loan => loan.Id),

                "FAMILY" when isAdmin => sortDescending
                    ? loansQuery.OrderByDescending(loan => loan.User.Family)
                        .ThenBy(loan => loan.Id)
                    : loansQuery.OrderBy(loan => loan.User.Family)
                        .ThenBy(loan => loan.Id),

                "EMAIL" when isAdmin => sortDescending
                    ? loansQuery.OrderByDescending(loan => loan.User.Email)
                        .ThenBy(loan => loan.Id)
                    : loansQuery.OrderBy(loan => loan.User.Email)
                        .ThenBy(loan => loan.Id),

                _ => loansQuery
                    .OrderByDescending(loan => loan.StartDate)
                    .ThenBy(loan => loan.Id)
            };
        }


    }
}
