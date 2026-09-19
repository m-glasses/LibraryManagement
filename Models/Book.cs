using LibraryManagement.ViewModels.Validation;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models
{
    public class Book : BaseEntity
    {
        public Book()
        {
            BookCopies = new List<BookCopy>();
            Reservations = new List<Reservation>();
        }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        [StringLength(100)]
        public string Author { get; set; }

        [Required]
        [StringLength(100)]
        public string Publisher { get; set; }

        [PublicationYearRange]
        public int PublicationYear { get; set; }

        public PublicationSeason? PublicationSeason { get; set; }

        [Range(1, 10000)]
        public int PageCount { get; set; }

        [Range(1, 1000)]
        public int Edition { get; set; }

        [Range(1, 100)]
        public int Volume { get; set; }

        //Navigation Property
        public virtual List<BookCopy> BookCopies { get; set; }
        public virtual List<Reservation> Reservations { get; set; }

    }
    public enum PublicationSeason
    {
        Spring,
        Summer,
        Autumn,
        Winter
    }
}
