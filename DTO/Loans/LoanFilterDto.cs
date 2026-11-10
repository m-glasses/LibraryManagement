
using LibraryManagement.Models;

namespace LibraryManagement.DTO.Loans
{
    public class LoanFilterDto
    {
        public string? SearchTerm { get; set; }
        public int? LoanId { get; set; }

        public LoanStatus? Status { get; set; }

        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }

        public DateTime? DueDateFrom { get; set; }
        public DateTime? DueDateTo { get; set; }

        public DateTime? ReturnDateFrom { get; set; }
        public DateTime? ReturnDateTo { get; set; }

        public string? SortBy { get; set; }
        public bool SortDescending { get; set; } = false;

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
