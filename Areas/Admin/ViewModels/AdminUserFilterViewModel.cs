using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Areas.Admin.ViewModels
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
    }
}