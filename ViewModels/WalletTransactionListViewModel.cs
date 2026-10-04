using LibraryManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.ViewModels
{
    public class WalletTransactionListViewModel
    {
        [Display(Name = "شناسه")]
        public int Id { get; set; }

        [Display(Name = "مبلغ")]
        public decimal Amount { get; set; }

        [Display(Name = "نوع تراکنش")]
        public TransactionType Type { get; set; }

        [Display(Name = "تاریخ تراکنش")]
        public DateTime CreatedAt { get; set; }
    }
}