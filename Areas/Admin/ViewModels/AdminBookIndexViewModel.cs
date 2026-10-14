using LibraryManagement.ViewModels;

namespace LibraryManagement.Areas.Admin.ViewModels
{
    public class AdminBookIndexViewModel
    {
        public List<BookListViewModel> Books { get; set; } = new();

        public AdminBookFilterViewModel Filter { get; set; } = new();

        public int TotalCount { get; set; }

        public int TotalPages { get; set; }
    }
}