using LibraryManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.ViewModels
{
    public class BookDetailsViewModel
    {
        public int Id { get; set; }

        [Display(Name = "عنوان کتاب")]
        public string Title { get; set; }

        [Display(Name = "نویسنده")]
        public string Author { get; set; }

        [Display(Name = "ناشر")]
        public string Publisher { get; set; }

        [Display(Name = "سال انتشار")]
        public int PublicationYear { get; set; }

        [Display(Name = "فصل انتشار")]
        public PublicationSeason? PublicationSeason { get; set; }

        [Display(Name = "تعداد صفحات")]
        public int PageCount { get; set; }

        [Display(Name = "ویرایش")]
        public int Edition { get; set; }

        [Display(Name = "جلد")]
        public int Volume { get; set; }

        [Display(Name = "تعداد نسخه")]
        public int TotalCopies { get; set; }

        [Display(Name = "نسخه‌های موجود")]
        public int AvailableCopies { get; set; }
    }
}