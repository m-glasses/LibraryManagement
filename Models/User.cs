using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models
{
    public class User : IdentityUser<int>
    {
        public User()
        {
            Loans = new List<Loan>();
            Reservations = new List<Reservation>();
            PaymentAttempts = new List<PaymentAttempt>();
        }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Family { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [StringLength(50)]
        public string FatherName { get; set; }


        public EducationLevel Education { get; set; }


        [StringLength(300)]
        public string? Address { get; set; }

        public Gender Gender { get; set; }

        public bool IsActive { get; set; } = true;

        //Navigation Property

        public virtual List<Loan> Loans { get; set; }
        public virtual List<Reservation> Reservations { get; set; }
        public Wallet Wallet { get; set; }
        public virtual List<PaymentAttempt> PaymentAttempts { get; set; }
    }
    public enum Gender
    {
        [Display(Name = "مرد")]
        Male,

        [Display(Name = "زن")]
        Female
    }

    public enum EducationLevel
    {
        [Display(Name = "راهنمایی")]
        MiddleSchool,

        [Display(Name = "دبیرستان")]
        HighSchool,

        [Display(Name = "دیپلم")]
        Diploma,

        [Display(Name = "کاردانی")]
        Associate,

        [Display(Name = "کارشناسی")]
        Bachelor,

        [Display(Name = "کارشناسی ارشد")]
        Master,

        [Display(Name = "دکتری")]
        PhD
    }
}
