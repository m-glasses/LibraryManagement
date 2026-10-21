using LibraryManagement.Areas.Admin.ViewModels.BookCopies;

namespace LibraryManagement.DTO.BookCopies
{
    public class BookCopiesFilterDto
    {
        public string? SearchTerm { get; set; }
        public int? BookId { get; set; }
        public BookCopyStatus? Status { get; set; }
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; } = false;
        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 20;
    }
}
