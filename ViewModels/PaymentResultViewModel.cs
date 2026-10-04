using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.ViewModels
{
    public class PaymentResultViewModel
    {
        [Display(Name = "وضعیت پرداخت")]
        public bool IsSuccess { get; set; }

        [Display(Name = "پیام")]
        public string Message { get; set; } = string.Empty;

        [Display(Name = "مبلغ")]
        public decimal Amount { get; set; }

        [Display(Name = "شماره پیگیری")]
        public long? ReferenceId { get; set; }
    }
}