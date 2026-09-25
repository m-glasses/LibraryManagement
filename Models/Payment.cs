namespace LibraryManagement.Models
{
    public class Payment : BaseEntity
    {
        public int LoanId { get; set; }

        public decimal Amount { get; set; }
        public DateTime PaidAt { get; set; }
        public int WalletTransactionId { get; set; }

        // Navigation Property
        public virtual WalletTransaction WalletTransaction { get; set; }
        public virtual Loan Loan { get; set; }
    }
}