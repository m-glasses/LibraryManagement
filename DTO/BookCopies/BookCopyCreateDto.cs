namespace LibraryManagement.DTO.BookCopies
{
    public class BookCopyCreateDto
    {
        public int BookId { get; set; }

        public string InventoryNumber { get; set; } = string.Empty;
    }
}
