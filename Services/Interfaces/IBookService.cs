using LibraryManagement.DTO.Book;
using LibraryManagement.Models;
using LibraryManagement.ViewModels;

namespace LibraryManagement.Services.Interfaces
{
    public interface IBookService : IGenericService<Book>
    {
        List<BookListViewModel> GetBookList(BookFilterDto filter);
        BookDetailsViewModel? GetBookDetails(int id);
        BookListResult GetAllBooks(BookListQueryDto input);

    }
}
