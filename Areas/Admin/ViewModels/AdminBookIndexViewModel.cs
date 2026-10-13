using LibraryManagement.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Areas.Admin.ViewModels
{
    public class AdminBookIndexViewModel
    {
        public List<BookListViewModel> Books { get; set; } = new();

        [Display(Name = "جستجوی کتاب")]
        public string? SearchTerm { get; set; }

        public int CurrentPage { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public int TotalPages { get; set; }

    }
}
