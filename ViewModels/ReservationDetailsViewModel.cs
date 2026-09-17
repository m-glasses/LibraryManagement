using System.ComponentModel.DataAnnotations;
using LibraryManagement.Models;

namespace LibraryManagement.ViewModels
{
    public class ReservationDetailsViewModel
    {
        public int Id { get; set; }

        public int BookId { get; set; }

        [Display(Name = "عنوان کتاب")]
        public string BookTitle { get; set; }

        [Display(Name = "تاریخ رزرو")]
        public DateTime ReservedAt { get; set; }

        [Display(Name = "تاریخ انقضا")]
        public DateTime ExpiresAt { get; set; }

        [Display(Name = "وضعیت")]
        public ReservationStatus ReservationStatus { get; set; }
    }
}