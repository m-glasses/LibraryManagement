using LibraryManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Areas.Admin.ViewModels.BookCopies
{
    public class AdminBookCopyDetailsViewModel
    {
        [Display(Name = "شناسه نسخه")]
        public int Id { get; set; }

        [Display(Name = "شناسه کتاب")]
        public int BookId { get; set; }

        [Display(Name = "شماره اموال")]
        public string InventoryNumber { get; set; } = string.Empty;

        [Display(Name = "وضعیت نسخه")]
        public BookCopyStatus? Status { get; set; }

        [Display(Name = "عنوان کتاب")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "نویسنده")]
        public string Author { get; set; } = string.Empty;

        [Display(Name = "ناشر")]
        public string Publisher { get; set; } = string.Empty;

        [Display(Name = "سال انتشار")]
        public int PublicationYear { get; set; }

        [Display(Name = "فصل انتشار")]
        public PublicationSeason? PublicationSeason { get; set; }

        [Display(Name = "تعداد صفحات")]
        public int PageCount { get; set; }

        [Display(Name = "نوبت چاپ")]
        public int Edition { get; set; }

        [Display(Name = "شماره جلد")]
        public int Volume { get; set; }

        [Display(Name = "آخرین امانت‌گیرنده")]
        public string? LastBorrowerName { get; set; }

        [Display(Name = "تاریخ شروع امانت")]
        public DateTime? LastLoanStartDate { get; set; }

        [Display(Name = "موعد بازگشت")]
        public DateTime? LastLoanDueDate { get; set; }
    }
}