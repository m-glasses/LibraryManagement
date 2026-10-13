using LibraryManagement.Models;

public class RegisterDto
{
    public string Name { get; set; } = null!;
    public string Family { get; set; } = null!;
    public string? FatherName { get; set; }
    public DateTime DateOfBirth { get; set; }

    public EducationLevel? Education { get; set; }
    public Gender? Gender { get; set; }

    public string? Address { get; set; }
    public string UserName { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string Password { get; set; } = null!;
}