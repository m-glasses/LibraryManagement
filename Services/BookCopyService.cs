using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.Services.Interfaces;
using System.Net;

namespace LibraryManagement.Services
{
    public class BookCopyService : GenericService<BookCopy>,IBookCopyService
    {
        private readonly LibraryDbContext _context;
        public BookCopyService(LibraryDbContext context): base(context) 
        {
            _context = context;
        }
        
    }
}
