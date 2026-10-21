using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.DTO.BookCopies
{
    public class BookCopyEditDto
    {
        public int Id { get; set; }

        public string InventoryNumber { get; set; } = string.Empty;
    }
}
