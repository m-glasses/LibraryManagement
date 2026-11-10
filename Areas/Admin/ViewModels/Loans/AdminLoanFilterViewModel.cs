using LibraryManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Areas.Admin.ViewModels.Loans
{
    public class AdminLoanFilterViewModel
    {
        [Display(Name = "عبارت جستجو")]
        public string? SearchTerm { get; set; }

        [Display(Name = "وضعیت امانت")]
        public LoanStatus? Status { get; set; }

        [Display(Name = "تاریخ شروع امانت از")]
        public DateTime? StartDateFrom { get; set; }

        [Display(Name = "تاریخ شروع امانت تا")]
        public DateTime? StartDateTo { get; set; }

        [Display(Name = "سررسید از")]
        public DateTime? DueDateFrom { get; set; }

        [Display(Name = "سررسید تا")]
        public DateTime? DueDateTo { get; set; }

        [Display(Name = "تاریخ بازگشت از")]
        public DateTime? ReturnDateFrom { get; set; }

        [Display(Name = "تاریخ بازگشت تا")]
        public DateTime? ReturnDateTo { get; set; }

        [Display(Name = "مرتب‌سازی بر اساس")]
        public string? SortBy { get; set; }

        [Display(Name = "مرتب‌سازی نزولی")]
        public bool SortDescending { get; set; } = false;

        [Display(Name = "شماره صفحه")]
        [Range(1, int.MaxValue, ErrorMessage = "شماره صفحه باید حداقل ۱ باشد.")]
        public int Page { get; set; } = 1;

        [Display(Name = "تعداد امانت در هر صفحه")]
        [Range(1, 50, ErrorMessage = "تعداد امانت در هر صفحه باید بین ۱ تا ۵۰ باشد.")]
        public int PageSize { get; set; } = 20;
    }
}