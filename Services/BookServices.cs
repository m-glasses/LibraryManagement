using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.Services.Interfaces;

namespace LibraryManagement.Services
{
    public class BookService : GenericService<Book> , IBookService
    {
        private readonly LibraryDbContext _context;
        public BookService(LibraryDbContext context): base(context)
        {
            _context = context;
        }
       
    }
}
