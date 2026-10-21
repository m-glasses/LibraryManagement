using LibraryManagement.Areas.Admin.ViewModels.BookCopies;
using LibraryManagement.Models;

namespace LibraryManagement.DTO.BookCopies
{
    public class AdminBookCopyDetailDto
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string InventoryNumber { get; set; } = string.Empty;
        public BookCopyStatus? Status { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public int PublicationYear { get; set; }
        public PublicationSeason? PublicationSeason { get; set; }
        public int PageCount { get; set; }
        public int Edition { get; set; }
        public int Volume { get; set; }

        public string? LastBorrowerName { get; set; }
        public DateTime? LastLoanStartDate { get; set; }
        public DateTime? LastLoanDueDate { get; set; }
    }
}
