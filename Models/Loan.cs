using System.ComponentModel.DataAnnotations;

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

        [Range(0,int.MaxValue)]
        public int RenewalCount { get; set; }
        public decimal DailyRate { get; set; }
        public decimal LateFeePerDay { get; set; }
        public int LateDays { get; set; }
        public decimal CalculatedAmount { get; set; }
        public decimal LateFee { get; set; }
        public decimal FinalAmount { get; set; }

        [StringLength(500)]
        public string? AdminNote { get; set; }

        //Navigation Property
        public virtual User User { get; set; }
        public virtual BookCopy BookCopy { get; set; }
    }
    public enum LoanStatus
    {
        Active,
        ReturnPending,
        Completed
    }
}
