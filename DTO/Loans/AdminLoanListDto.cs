
using LibraryManagement.Models;

namespace LibraryManagement.DTO.Loans
{
    public class AdminLoanListDto
    {
        public int LoanId { get; set; }
        public string UserFullName { get; set; } = string.Empty;
        public string Email {  get; set; } = string.Empty;
        public string BookTitle { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string InventoryNumber { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public LoanStatus Status { get; set; }
        public decimal FinalAmount { get; set; }
    }
}
