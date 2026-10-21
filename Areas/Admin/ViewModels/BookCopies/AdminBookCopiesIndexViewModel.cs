namespace LibraryManagement.Areas.Admin.ViewModels.BookCopies
{
    public class AdminBookCopiesIndexViewModel
    {
        public List<AdminBookCopiesListViewModel> BookCopies { get; set; } = new();

        public AdminBookCopiesFilterViewModel Filter { get; set; } = new();

        public int TotalCount { get; set; }

        public int TotalPages { get; set; }
    }
}