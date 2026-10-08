using LibraryManagement.Models;

namespace LibraryManagement.DTO
{
    public class UserDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Family { get; set; }
        public string FatherName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public EducationLevel Education { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        public string? Address { get; set; }
        public decimal WalletBalance { get; set; }
        public bool IsActive { get; set; }
    }
}