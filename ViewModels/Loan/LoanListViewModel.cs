using LibraryManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.ViewModels.Loan
{
    public class LoanListViewModel
    {
        [Display(Name = "شناسه امانت")]
        public int LoanId { get; set; }

        [Display(Name = "عنوان کتاب")]
        public string BookTitle { get; set; } = string.Empty;

        [Display(Name = "نویسنده")]
        public string Author { get; set; } = string.Empty;

        [Display(Name = "شماره اموال")]
        public string InventoryNumber { get; set; } = string.Empty;

        [Display(Name = "تاریخ شروع امانت")]
        public DateTime StartDate { get; set; }

        [Display(Name = "تاریخ سررسید")]
        public DateTime DueDate { get; set; }

        [Display(Name = "تاریخ بازگشت")]
        public DateTime? ReturnDate { get; set; }

        [Display(Name = "وضعیت امانت")]
        public LoanStatus Status { get; set; }

        [Display(Name = "مبلغ نهایی")]
        public decimal FinalAmount { get; set; }
    }
}