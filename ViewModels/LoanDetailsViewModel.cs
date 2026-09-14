using LibraryManagement.Models;

namespace LibraryManagement.ViewModels
{
    public class LoanDetailsViewModel
    {
        public int Id { get; set; }
        public string BookTitle { get; set; }
        public string Author { get; set; }
        public string InventoryNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public LoanStatus LoanStatus { get; set; }
    }
}
