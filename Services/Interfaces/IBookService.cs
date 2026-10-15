using LibraryManagement.DTO.Book;
using LibraryManagement.DTO.Book.Admin;
using LibraryManagement.DTO.Book.User;
using LibraryManagement.Models;

namespace LibraryManagement.Services.Interfaces
{
    public interface IBookService : IGenericService<Book>
    {
        BookListResult GetBookList(BookListQueryDto input);

        UserBookDetailDto? GetBookDetails(int id);

        AdminBookDetailsDto? AdminGetBookDetails(int id);
    }
}