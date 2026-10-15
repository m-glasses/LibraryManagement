namespace LibraryManagement.Areas.Admin.ViewModels.Book
{
    public class BookIndexViewModel
    {
        public List<AdminBookListViewModel> Books { get; set; } = new();

        public AdminBookFilterViewModel Filter { get; set; } = new();

        public int TotalCount { get; set; }

        public int TotalPages { get; set; }
    }
}

