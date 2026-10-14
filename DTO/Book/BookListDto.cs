namespace LibraryManagement.DTO.Book
{
    public class BookListDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public string Publisher { get; set; }

        public int PublicationYear { get; set; }

        public int TotalCopies { get; set; }

        public int AvailableCopies { get; set; }
        public int ActiveReservations { get; set; }

    }
}
