namespace LibraryManagement.Models
{
    public class Wallet : BaseEntity
    {
        public Wallet()
        {
            WalletTransactions = new List<WalletTransaction>();
        }
        public int UserId { get; set; }
        public decimal Balance { get; set; }

        //Navigation Property
        public User User { get; set; }
        public List<WalletTransaction> WalletTransactions { get; set; }

    }
}
