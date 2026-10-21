
using LibraryManagement.Areas.Admin.ViewModels.BookCopies;

namespace LibraryManagement.DTO.BookCopies
{
    public class BookCopiesListDto
    {
        public int Id { get; set; }
        public int BookID { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Author { get; set; } = string.Empty;

        public string Publisher { get; set; } = string.Empty;

        public string InventoryNumber { get; set; } = string.Empty;

        public BookCopyStatus? Status { get; set; }
        public string? LastBorrowerName { get; set; }

        public DateTime? LastLoanStartDate { get; set; }

        public DateTime? LastLoanDueDate { get; set; }
    }
}
