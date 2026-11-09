using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibraryManagement.Areas.Admin.ViewModels.BookCopies
{
    public class BookCopyCreateViewModel
    {
        [Display(Name = "کتاب")]
        public int? BookId { get; set; }

        [Display(Name = "شماره اموال")]
        [Required(ErrorMessage = "وارد کردن شماره اموال الزامی است.")]
        [StringLength(50, ErrorMessage = "شماره اموال نمی‌تواند بیشتر از ۵۰ کاراکتر باشد.")]
        public string InventoryNumber { get; set; } = string.Empty;

        [Display(Name = "عنوان کتاب")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "نویسنده")]
        public string Author { get; set; } = string.Empty;

        public List<SelectListItem> Books { get; set; } = [];

        public bool IsBookPreselected { get; set; }
    }
}