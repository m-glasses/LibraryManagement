using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Areas.Admin.ViewModels
{
    public class AdminUserIndexViewModel
    {
        public List<UserListViewModel> Users { get; set; } = new();

        [Display(Name = "جستجوی کاربر")]
        public string? SearchTerm { get; set; }

        [Display(Name = "نقش کاربر")]
        public string? Role { get; set; }

        [Display(Name = "وضعیت بدهی")]
        public bool? IsDebtor { get; set; }

        public int CurrentPage { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public int TotalPages { get; set; }
    }
}