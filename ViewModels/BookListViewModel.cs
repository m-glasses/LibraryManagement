using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.ViewModels
{
    public class BookListViewModel
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

        [Display(Name = "تعداد نسخه")]
        public int TotalCopies { get; set; }

        [Display(Name = "نسخه‌های موجود")]
        public int AvailableCopies { get; set; }
    }
}