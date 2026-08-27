using System.Transactions;

namespace LibraryManagement.Models
{
    public class WalletTransaction : BaseEntity
    {
        public int WalletId { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public DateTime CreatedAt { get; set; }

        //Navigation Property
        public Wallet Wallet { get; set; }
    }
    public enum TransactionType
    {
        Deposit,
        Withdrawal
    }
}
