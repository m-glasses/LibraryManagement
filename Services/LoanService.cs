using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Services
{
    public class LoanService : GenericService<Loan>, ILoanService
    {
        private readonly LibraryDbContext _context;
        public LoanService(LibraryDbContext context) : base(context)
        {
            _context = context;
        }

        public BookCopy FindAvailableBookCopy(int bookId)
        {
            if (!_context.Books.Any(b => b.Id == bookId))
            {
                throw new InvalidOperationException("Book not found");
            }

            var bookCopy = _context.BookCopies
                .FirstOrDefault(bc =>
                    bc.BookId == bookId &&
                    !bc.Loans.Any(l => l.LoanStatus == LoanStatus.Active));

            if (bookCopy == null)
            {
                throw new InvalidOperationException("Book not Available");
            }

            return bookCopy;
        }


        public Loan Borrow(int userId, int bookCopyId)
        {
            var settings = _context.LibrarySettings.SingleOrDefault();
            if (settings == null)
            {
                throw new InvalidOperationException("Library settings not found.");
            }


            if (!_context.Users.Any(u => u.Id == userId))
            {
                throw new InvalidOperationException("User not found");
            }


            if (!_context.BookCopies.Any(bc => bc.Id == bookCopyId))
            {
                throw new InvalidOperationException("Book Copy not found");
            }


            if (_context.Loans.Any(l => l.BookCopyId == bookCopyId && l.LoanStatus == LoanStatus.Active))
            {
                throw new InvalidOperationException("Book copy is already on loan");
            }

            var startDate = DateTime.Now;
            var loan = new Loan()
            {
                UserId = userId,
                BookCopyId = bookCopyId,
                StartDate = startDate,
                DueDate = startDate.AddDays(settings.LoanDurationDays),
                LoanStatus = LoanStatus.Active,
                RenewalCount = 0,
                LateFeePerDay = settings.LateFeePerDay,
                DailyRate = settings.DailyRate,
            };
            _context.Loans.Add(loan);
            _context.SaveChanges();
            return loan;
        }

        public bool RequestReturn(int loanId)
        {
            var loan = _context.Loans.FirstOrDefault(l => l.Id == loanId);

            if (loan == null)
            {
                return false;
            }

            if (loan.LoanStatus != LoanStatus.Active)
            {
                return false;

            }

            loan.LoanStatus = LoanStatus.ReturnPending;
            _context.SaveChanges();
            return true;
        }

        public bool ConfirmReturn(int loanId, DateTime returnDate)
        {
            var loan = _context.Loans.FirstOrDefault(l => l.Id == loanId);

            if (loan == null)
            {
                return false;
            }

            if (loan.LoanStatus != LoanStatus.Active && loan.LoanStatus != LoanStatus.ReturnPending)
            {
                return false;
            }

            if (returnDate < loan.StartDate || returnDate > DateTime.Now)
            {
                return false;
            }

            loan.ReturnDate = returnDate;

            var normalDays = Math.Min(
                (returnDate - loan.StartDate).Days,
                (loan.DueDate - loan.StartDate).Days);

            loan.CalculatedAmount = normalDays * loan.DailyRate;

            loan.LateDays = Math.Max
                (0, (returnDate - loan.DueDate).Days);

            loan.LateFee = loan.LateDays * loan.LateFeePerDay;

            loan.FinalAmount = loan.LateFee + loan.CalculatedAmount;
            
                loan.LoanStatus = LoanStatus.Completed;
                _context.SaveChanges();
                return true;
            
        }

        public bool Renew(int loanId)
        {
            var loan = _context.Loans.FirstOrDefault(l => l.Id == loanId);

            if (loan == null)
            {
                return false;
            }

            if(loan.LoanStatus != LoanStatus.Active)
            {
                return false;
            }

            var librarySetting = _context.LibrarySettings.SingleOrDefault();

            if (librarySetting == null)
            {
                return false;
            }

            if(loan.RenewalCount >= librarySetting.MaxRenewalCount)
            {
                return false;
            }

            loan.DueDate = loan.DueDate.AddDays(librarySetting.RenewalDurationDays);

            loan.RenewalCount++;

            _context.SaveChanges();
            return true;

        }

        public Loan? GetDetailsById(int id)
        {
            return _context.Loans
                .Include(l => l.BookCopy)
                .ThenInclude(bc => bc.Book)
                .FirstOrDefault(l => l.Id == id);
        }
    }
}
