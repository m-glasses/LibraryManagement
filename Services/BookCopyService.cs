using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.Services.Interfaces;
using System.Net;

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
                .Where(bc => !bc.Loans.Any(l =>
                    l.LoanStatus == LoanStatus.Active ||
                    l.LoanStatus == LoanStatus.ReturnPending))
                    .ToList();
            return bookCopies;

        }
    }
}
