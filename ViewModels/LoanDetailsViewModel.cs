using LibraryManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.ViewModels
{
    public class LoanDetailsViewModel
    {
        public int Id { get; set; }

        [Display(Name = "عنوان کتاب")]
        public string BookTitle { get; set; }

        [Display(Name = "نویسنده")]
        public string Author { get; set; }

        [Display(Name = "شماره نسخه")]
        public string InventoryNumber { get; set; }

        [Display(Name = "تاریخ امانت")]
        public DateTime StartDate { get; set; }

        [Display(Name = "تاریخ سررسید")]
        public DateTime DueDate { get; set; }

        [Display(Name = "وضعیت")]
        public LoanStatus LoanStatus { get; set; }
    }
}