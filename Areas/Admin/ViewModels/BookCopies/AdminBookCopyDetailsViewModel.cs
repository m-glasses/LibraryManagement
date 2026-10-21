using LibraryManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Areas.Admin.ViewModels.BookCopies
{
    public class AdminBookCopyDetailsViewModel
    {
        [Display(Name = "شناسه")]
        public int Id { get; set; }

        [Display(Name = "شناسه کتاب")]
        public int BookId { get; set; }

        [Display(Name = "عنوان")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "شماره اموال")]
        public string InventoryNumber { get; set; } = string.Empty;

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

        [Display(Name = "جلد")]
        public int Volume { get; set; }

        [Display(Name = "وضعیت نسخه")]
        public string Status { get; set; } = string.Empty;

    }
}
