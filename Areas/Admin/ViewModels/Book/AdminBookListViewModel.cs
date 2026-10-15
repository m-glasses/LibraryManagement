using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Areas.Admin.ViewModels.Book
{
    public class AdminBookListViewModel
    {
        [Display(Name = "شناسه کتاب")]
        public int Id { get; set; }

        [Display(Name = "عنوان کتاب")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "نویسنده")]
        public string Author { get; set; } = string.Empty;

        [Display(Name = "ناشر")]
        public string Publisher { get; set; } = string.Empty;

        [Display(Name = "سال انتشار")]
        public int PublicationYear { get; set; }

        [Display(Name = "تعداد کل نسخه‌ها")]
        public int TotalCopies { get; set; }

        [Display(Name = "نسخه‌های موجود")]
        public int AvailableCopies { get; set; }

        [Display(Name = "تعداد رزروهای فعال")]
        public int ActiveReservations { get; set; }
    }
}
