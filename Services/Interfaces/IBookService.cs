using LibraryManagement.Models;
using LibraryManagement.ViewModels;

namespace LibraryManagement.Services.Interfaces
{
    public interface IBookService : IGenericService<Book>
    {
        List<BookListViewModel> GetBookList();
        BookDetailsViewModel? GetBookDetails(int id);

    }
}
