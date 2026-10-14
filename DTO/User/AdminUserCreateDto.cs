using LibraryManagement.Models;

namespace LibraryManagement.DTO.User
{
    public class AdminUserCreateDto
    {
        public string Name { get; set; }
        public string Family { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string FatherName { get; set; }
        public EducationLevel? Education { get; set; }
        public string? Address { get; set; }
        public Gender? Gender { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
    }
}
