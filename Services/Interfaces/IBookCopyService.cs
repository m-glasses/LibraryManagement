using LibraryManagement.DTO.BookCopies;
using LibraryManagement.Models;

namespace LibraryManagement.Services.Interfaces
{
    public interface IBookCopyService : IGenericService<BookCopy>
    {
        List<BookCopy> GetAvailableCopies();
        BookCopiesListResult GetBookCopiesList(BookCopiesFilterDto input);
    }
}
