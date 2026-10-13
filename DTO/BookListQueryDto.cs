namespace LibraryManagement.DTO
{
    public class BookListQueryDto
    {
        public string? SearchTerm { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 20;

        public bool? Availability { get; set; }

        public string? SortBy { get; set; }

        public bool SortDescending { get; set; } = false;
    }
}