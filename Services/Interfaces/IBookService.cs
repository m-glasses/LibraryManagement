using LibraryManagement.Models;
using LibraryManagement.ViewModels;

namespace LibraryManagement.Services.Interfaces
{
    public interface IBookService : IGenericService<Book>
    {
        public List<BookListViewModel> GetBookList(); 
        public BookListViewModel GetBookDetails(int id);
        
    }
}
