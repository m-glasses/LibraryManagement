namespace LibraryManagement.Models
{
    public class Loan : BaseEntity
    {
        public int UserId { get; set; }
        public int BookCopyId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public LoanStatus LoanStatus { get; set; }
        public int RenewalCount { get; set; }
        public decimal DailyRate { get; set; }
        public decimal LateFeePerDay { get; set; }
        public decimal CalculatedAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public string? AdminNote { get; set; }
    }
    public enum LoanStatus
    {
        Active,
        ReturnPending,
        Completed
    }
}
