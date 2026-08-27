using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models
{
    public class User : BaseEntity
    {
        public User() 
        {
            Loans = new List<Loan>();
            Reservations = new List<Reservation>();
        }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string  Family { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [StringLength(50)]
        public string FatherName { get; set; }


        public EducationLevel Education { get; set; }

        [Required]
        [StringLength(50)]
        public string UserName { get; set; }

        [Required]
        [StringLength(50)]
        public string Password { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [StringLength(300)]
        public string? Address { get; set; }

        public Gender Gender { get; set; }

        public UserRole Role { get; set; }

        //Navigation Property

        public virtual List<Loan> Loans { get; set; }
        public virtual List<Reservation> Reservations { get; set; }
        public Wallet Wallet { get; set; }
    }
    public enum UserRole
    {
        User ,
        Admin
    }
    public enum Gender
    {
        Male , 
        Female
    }

    public enum EducationLevel
    {
        MiddleSchool,
        HighSchool,
        Diploma,
        Associate,
        Bachelor,
        Master,
        PhD
    }
}
