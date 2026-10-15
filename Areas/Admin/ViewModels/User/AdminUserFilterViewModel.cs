using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Areas.Admin.ViewModels.User
{
    public class AdminUserFilterViewModel
    {
        [Display(Name = "جستجو")]
        public string? SearchTerm { get; set; }

        [Display(Name = "نقش")]
        public string? Role { get; set; }

        [Display(Name = "وضعیت مالی")]
        public bool? IsDebtor { get; set; }

        [Display(Name = "صفحه")]
        public int Page { get; set; } = 1;

        [Display(Name = "تعداد در هر صفحه")]
        public int PageSize { get; set; } = 20;

        [Display(Name = "مرتب‌سازی بر اساس")]
        public string? SortBy { get; set; }

        [Display(Name = "مرتب‌سازی نزولی")]
        public bool SortDescending { get; set; } = false;
    }
}