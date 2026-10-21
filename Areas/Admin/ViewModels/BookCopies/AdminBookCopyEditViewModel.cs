
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Areas.Admin.ViewModels.BookCopies
{
    public class AdminBookCopyEditViewModel
    {
        [Display(Name = "شناسه نسخه")]
        [Range(1, int.MaxValue, ErrorMessage = "شناسه نسخه معتبر نیست.")]
        public int Id { get; set; }

        [Display(Name = "شماره اموال")]
        [Required(ErrorMessage = "وارد کردن شماره اموال الزامی است.")]
        [StringLength(50, ErrorMessage = "شماره اموال نمی‌تواند بیشتر از ۵۰ کاراکتر باشد.")]
        public string InventoryNumber { get; set; } = string.Empty;
    }
}
