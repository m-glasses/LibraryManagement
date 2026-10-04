using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.ViewModels
{
    public class WalletViewModel
    {
        [Display(Name = "موجودی کیف پول")]
        public decimal Balance { get; set; }

        [Display(Name = "وضعیت کیف پول")]
        public bool IsBlocked => Balance < 0;

        //Navigation Property
        public List<WalletTransactionListViewModel> Transactions { get; set; } = new();
    }
}