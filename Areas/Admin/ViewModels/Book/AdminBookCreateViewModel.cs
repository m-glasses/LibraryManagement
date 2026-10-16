using LibraryManagement.Models;
using LibraryManagement.ViewModels.Validation;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Areas.Admin.ViewModels.Book
{
    public class AdminBookCreateViewModel
    {

        [Display(Name = "عنوان کتاب")]
        [Required(ErrorMessage = "وارد کردن عنوان کتاب الزامی است.")]
        [StringLength(200, ErrorMessage = "عنوان کتاب نمی‌تواند بیشتر از {1} کاراکتر باشد.")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "نویسنده")]
        [Required(ErrorMessage = "وارد کردن نام نویسنده الزامی است.")]
        [StringLength(100, ErrorMessage = "نام نویسنده نمی‌تواند بیشتر از {1} کاراکتر باشد.")]
        public string Author { get; set; } = string.Empty;

        [Display(Name = "ناشر")]
        [Required(ErrorMessage = "وارد کردن نام ناشر الزامی است.")]
        [StringLength(100, ErrorMessage = "نام ناشر نمی‌تواند بیشتر از {1} کاراکتر باشد.")]
        public string Publisher { get; set; } = string.Empty;

        [Display(Name = "سال انتشار")]
        [PublicationYearRange]
        public int PublicationYear { get; set; }

        [Display(Name = "فصل انتشار")]
        public PublicationSeason? PublicationSeason { get; set; }

        [Display(Name = "تعداد صفحات")]
        [Range(1, 10000, ErrorMessage = "تعداد صفحات باید بین {1} و {2} باشد.")]
        public int PageCount { get; set; }

        [Display(Name = "ویرایش")]
        [Range(1, 1000, ErrorMessage = "شماره ویرایش باید بین {1} و {2} باشد.")]
        public int Edition { get; set; }

        [Display(Name = "جلد")]
        [Range(1, 1000, ErrorMessage = "شماره جلد باید بین {1} و {2} باشد.")]
        public int Volume { get; set; }
    }
}
