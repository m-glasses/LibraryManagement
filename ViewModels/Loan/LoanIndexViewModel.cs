namespace LibraryManagement.ViewModels.Loan
{
    public class LoanIndexViewModel
    {
        public List<LoanListViewModel> Loans { get; set; } = new();

        public LoanFilterViewModel Filter { get; set; } = new();

        public int TotalCount { get; set; }

        public int TotalPages { get; set; }
    }
}
