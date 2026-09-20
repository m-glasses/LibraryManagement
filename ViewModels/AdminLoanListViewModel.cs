using LibraryManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.ViewModels
{
    public class AdminLoanListViewModel
    {
        [Display(Name = "شناسه")]
        public int Id { get; set; }

        [Display(Name = "کاربر")]
        public string UserFullName { get; set; }

        [Display(Name = "عنوان کتاب")]
        public string BookTitle { get; set; }

        [Display(Name = "شماره نسخه")]
        public string InventoryNumber { get; set; }

        [Display(Name = "تاریخ امانت")]
        public DateTime LoanDate { get; set; }

        [Display(Name = "تاریخ سررسید")]
        public DateTime DueDate { get; set; }

        [Display(Name = "تاریخ برگشت")]
        public DateTime? ReturnDate { get; set; }

        [Display(Name = "وضعیت")]
        public LoanStatus LoanStatus { get; set; }

        [Display(Name = "مبلغ نهایی")]
        public decimal FinalAmount { get; set; }
    }
}