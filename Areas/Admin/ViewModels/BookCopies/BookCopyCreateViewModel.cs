using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Areas.Admin.ViewModels.BookCopies
{
    public class BookCopyCreateViewModel
    {
        [Display(Name = "کتاب")]
        [Range(1, int.MaxValue, ErrorMessage = "انتخاب کتاب الزامی است.")]
        public int BookId { get; set; }

        [Display(Name = "شماره اموال")]
        [Required(ErrorMessage = "وارد کردن شماره اموال الزامی است.")]
        [StringLength(50, ErrorMessage = "شماره اموال نمی‌تواند بیشتر از ۵۰ کاراکتر باشد.")]
        public string InventoryNumber { get; set; } = string.Empty;

        [Display(Name = "عنوان کتاب")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "نویسنده")]
        public string Author { get; set; } = string.Empty;
    }
}
