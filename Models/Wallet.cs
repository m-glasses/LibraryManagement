namespace LibraryManagement.Models
{
    public class Wallet : BaseEntity
    {
        public int  UserId { get; set; }
        public decimal Balance { get; set; }

    }
}
