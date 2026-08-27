namespace LibraryManagement.Models
{
    public class Reservation : BaseEntity
    {
        public int UserId { get; set; }
        public int BookId { get; set; }
        public DateTime ReservedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public ReservationStatus ReservationStatus { get; set; }
    }
    public enum ReservationStatus
    {
        Active,
        Completed,
        Cancelled,
        Expired
    }
}
