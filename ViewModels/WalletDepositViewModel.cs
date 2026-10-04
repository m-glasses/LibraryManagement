using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.ViewModels
{
    public class WalletDepositViewModel
    {
        [Display(Name = "مبلغ شارژ")]
        [Required(ErrorMessage = "مبلغ شارژ را وارد کنید.")]
        [Range(1000, 100000000, ErrorMessage = "مبلغ شارژ باید بین ۱,۰۰۰ تا ۱۰۰,۰۰۰,۰۰۰ تومان باشد.")]
        public decimal Amount { get; set; }
    }
}