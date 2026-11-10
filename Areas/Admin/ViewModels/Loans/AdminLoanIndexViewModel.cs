namespace LibraryManagement.Areas.Admin.ViewModels.Loans
{
    public class AdminLoanIndexViewModel
    {
        public List<AdminLoanListViewModel> Loans { get; set; } = new();

        public AdminLoanFilterViewModel Filter { get; set; } = new();

        public int TotalCount { get; set; }

        public int TotalPages { get; set; }
    }
}