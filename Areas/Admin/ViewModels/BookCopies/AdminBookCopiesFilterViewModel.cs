using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Areas.Admin.ViewModels.BookCopies
{
    public class AdminBookCopiesFilterViewModel
    {
        [Display(Name = "عبارت جستجو")]
        public string? SearchTerm { get; set; }

        [Display(Name = "شماره صفحه")]
        [Range(1, int.MaxValue, ErrorMessage = "شماره صفحه باید حداقل ۱ باشد.")]
        public int Page { get; set; } = 1;

        [Display(Name = "تعداد نسخه در هر صفحه")]
        [Range(1, 50, ErrorMessage = "تعداد نسخه در هر صفحه باید بین ۱ تا ۵۰ باشد.")]
        public int PageSize { get; set; } = 20;

        [Display(Name = "وضعیت موجودی نسخه")]
        public BookCopyStatus? Status { get; set; }

        [Display(Name = "مرتب‌سازی بر اساس")]
        public string? SortBy { get; set; }

        [Display(Name = "مرتب‌سازی نزولی")]
        public bool SortDescending { get; set; } = false;
    }
}
