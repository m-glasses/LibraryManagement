using LibraryManagement.Models;
using Microsoft.AspNetCore.Identity;
namespace LibraryManagement.Data
{
    public class SeedData
    {
        public static async Task Initialize(LibraryDbContext context , UserManager<User> userManager , RoleManager<IdentityRole<int>> roleManager)
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
                await context.SaveChangesAsync();
               
            }

            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole<int>("Admin"));
            }

            if(! await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole<int>("User"));
            }

            var admin = await userManager.FindByNameAsync("Admin");

            if (admin == null)
            {
                admin = new User()
                {
                    Name = "Admin",
                    Family = "Admin",
                    DateOfBirth = new DateTime(2000, 1, 1),
                    FatherName = "Admin",
                    Education = EducationLevel.Diploma,
                    UserName = "Admin",
                    PhoneNumber = "09121010101",
                    Gender = Gender.Male
                };

                var result = await userManager.CreateAsync(
                    admin,
                    "Admin@1234");
                if (!result.Succeeded)
                {
                    throw new Exception(
                        string.Join(", ", result.Errors.Select(e =>e.Description)));
                }
            }

            if(!await userManager.IsInRoleAsync(admin, "Admin"))
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }

            
        }
    }
}
