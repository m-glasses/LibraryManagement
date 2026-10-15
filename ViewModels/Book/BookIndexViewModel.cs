namespace LibraryManagement.ViewModels.Book
{
    public class BookIndexViewModel
    {
        public List<BookListViewModel> Books { get; set; } = new();

        public BookFilterViewModel Filter { get; set; } = new();

        public int TotalCount { get; set; }

        public int TotalPages { get; set; }
    }
}
