using LibraryManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.ViewModels
{
    public class AdminLoanDetailsViewModel
    {
        // Loan details

        [Display(Name = "شناسه")]
        public int Id { get; set; }

        [Display(Name = "تاریخ امانت")]
        public DateTime StartDate { get; set; }

        [Display(Name = "تاریخ سررسید")]
        public DateTime DueDate { get; set; }

        [Display(Name = "تاریخ بازگشت")]
        public DateTime? ReturnDate { get; set; }

        [Display(Name = "وضعیت")]
        public LoanStatus LoanStatus { get; set; }

        [Display(Name = "تعداد تمدید")]
        public int RenewalCount { get; set; }


        // User details

        [Display(Name = "شناسه کاربر")]
        public int UserId { get; set; }

        [Display(Name = "نام و نام خانوادگی")]
        public string FullName { get; set; }


        // Book details

        [Display(Name = "عنوان کتاب")]
        public string BookTitle { get; set; }

        [Display(Name = "شماره نسخه")]
        public string InventoryNumber { get; set; }


        // Financial details

        [Display(Name = "نرخ روزانه")]
        public decimal DailyRate { get; set; }

        [Display(Name = "نرخ جریمه روزانه")]
        public decimal LateFeePerDay { get; set; }

        [Display(Name = "روزهای تأخیر")]
        public int LateDays { get; set; }

        [Display(Name = "مبلغ محاسبه‌شده")]
        public decimal CalculatedAmount { get; set; }

        [Display(Name = "جریمه تأخیر")]
        public decimal LateFee { get; set; }

        [Display(Name = "مبلغ نهایی")]
        public decimal FinalAmount { get; set; }


        // Administrative details

        [Display(Name = "یادداشت مدیر")]
        public string? AdminNote { get; set; }
    }
}