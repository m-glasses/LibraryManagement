using LibraryManagement.Models;

namespace LibraryManagement.Areas.Admin.ViewModels
{
    public class AdminUserDetailsViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public EducationLevel Education { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        public string? Address { get; set; }
        public decimal WalletBalance { get; set; }
        public bool IsDebtor { get; set; }


    }
}
