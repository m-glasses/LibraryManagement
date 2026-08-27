using System.Transactions;

namespace LibraryManagement.Models
{
    public class WalletTransactio : BaseEntity
    {
        public int WalletId { get; set; }
        public decimal Amount { get; set; }
        public TransactionType MyProperty { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public enum TransactionType
    {
        Deposit,
        Withdrawal
    }
}
