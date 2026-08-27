using LibraryManagement.Models;
namespace LibraryManagement.Data
{
    public class SeedData
    {
        public static void Initialize(LibraryDbContext context)
        {
            if (!context.LibrarySettings.Any())
            {
                LibrarySettings librarySetting = new LibrarySettings()
                {
                    LoanDurationDays = 14,
                    DailyRate = 1000,
                    LateFeePerDay = 2000,
                    MaxRenewalCount = 2,
                    RenewalDurationDays = 7,
                    ReservationDurationDays = 7
                };
                context.LibrarySettings.Add(librarySetting);
               
            }

            if (!context.Users.Any(u=> u.Role == UserRole.Admin))
            {
                User user = new User()
                {
                    Name = "Admin",
                    Family = "Admin",
                    DateOfBirth = new DateTime(2000, 1, 1),
                    FatherName = "Admin",
                    Education = EducationLevel.Diploma,
                    UserName = "Admin",
                    Password = "1234",
                    PhoneNumber = "09121010101",
                    Gender = Gender.Male,
                    Role = UserRole.Admin,

                };
                context.Users.Add(user);
            }
            context.SaveChanges();
        }
    }
}
