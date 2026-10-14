namespace LibraryManagement.DTO.Book
{
    public class BookFilterDto
    {
        public string? SearchTerm { get; set; }
        public bool? HasAvailableCopies { get; set; }
        public bool? HasActiveReservations { get; set; }
        public string? SortBy { get; set; }

        public bool SortDescending { get; set; } = false;
    }
}
