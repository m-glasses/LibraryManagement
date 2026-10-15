using LibraryManagement.Models;

namespace LibraryManagement.DTO.Book.Admin
{
    public class AdminBookDetailsDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public int PublicationYear { get; set; }
        public PublicationSeason? PublicationSeason { get; set; }
        public int PageCount { get; set; }
        public int Edition { get; set; }
        public int Volume { get; set; }
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }
        public int ActiveReservations { get; set; }
    }
}