using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.Services.Interfaces;
using LibraryManagement.ViewModels;

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
                .SingleOrDefault();
        }

        public List<BookListViewModel> GetBookList()
        {
            return _context.Books
                .Select(book => new BookListViewModel
                {
                    Id = book.Id,
                    Title = book.Title,
                    Author = book.Author,
                    PublicationYear = book.PublicationYear,
                    Publisher = book.Publisher,
                    TotalCopies = book.BookCopies.Count(),
                    AvailableCopies = book.BookCopies.Count(bc => !bc.Loans.Any(l => l.LoanStatus == LoanStatus.Active || l.LoanStatus == LoanStatus.ReturnPending))
                })
                .ToList();
        }
    }
}
