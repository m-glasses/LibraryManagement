namespace LibraryManagement.Models
{
    public class LibrarySettings : BaseEntity
    {
        public int LoanDurationDays { get; set; }
        public decimal DailyRate { get; set; }
        public decimal LateFeePerDay { get; set; }
        public int MaxRenewalCount { get; set; }
        public int RenewalDurationDays { get; set; }
        public int ReservationDurationDays { get; set; }
    }
}
