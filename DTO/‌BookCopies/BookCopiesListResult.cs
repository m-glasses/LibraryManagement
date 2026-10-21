namespace LibraryManagement.DTO.BookCopies
{
    public class BookCopiesListResult
    {
        public List<BookCopiesListDto> BookCopies { get; set; } = new();

        public int CurrentPage { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public int TotalPages =>
            PageSize <= 0
                ? 0
                : (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
