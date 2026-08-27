using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models
{
    public class BookCopy : BaseEntity
    {
        public BookCopy() 
        {
            Loans = new List<Loan>();
        }
        public int  BookId { get; set; }

        [Required]
        [StringLength(50)]
        public string  InventoryNumber { get; set; }

        //Navigation Property
        public virtual Book Book { get; set; }
        public virtual List<Loan> Loans { get; set; }

    }
}
