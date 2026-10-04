namespace LibraryManagement.Models
{
    public class PaymentAttempt : BaseEntity
    {
        public int UserId { get; set; }

        public decimal Amount { get; set; }

        public string? Authority { get; set; }

        public PaymentStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }


        //Navigation property
        public virtual User User { get; set; }
    }

    public enum PaymentStatus
    {
        Pending,
        Verified,
        Failed
    }

}