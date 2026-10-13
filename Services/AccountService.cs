using LibraryManagement.Data;
using LibraryManagement.DTO;
using LibraryManagement.Exceptions;
using LibraryManagement.Models;
using LibraryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Win32;

namespace LibraryManagement.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly IWalletService _walletService;
        private readonly LibraryDbContext _context;
        private readonly SignInManager<User> _signInManager;
        private readonly IdentityErrorLocalizer _identityErrorLocalizer;

        public AccountService(UserManager<User> userManager , IWalletService walletService , LibraryDbContext context , IdentityErrorLocalizer identityErrorLocalizer , SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _walletService = walletService;
            _context = context;
            _identityErrorLocalizer = identityErrorLocalizer;
            _signInManager = signInManager;
        }



        public async Task LoginAsync(LoginDto input)
        {
            ArgumentNullException.ThrowIfNull(input);

            if (string.IsNullOrWhiteSpace(input.UserName) ||
                string.IsNullOrWhiteSpace(input.Password))
            {
                throw new InvalidOperationException(
                    "UserName or Password invalid.");
            }

            var user = await _userManager.FindByNameAsync(input.UserName);

            if (user is null)
            {
                throw new InvalidOperationException(
                    "UserName or Password invalid.");
            }

            if (!user.IsActive)
            {
                throw new InvalidOperationException(
                    "User is inactive.");
            }

            var result = await _signInManager.PasswordSignInAsync(
                input.UserName,
                input.Password,
                input.RememberMe,
                lockoutOnFailure: true);

            if (result.IsLockedOut)
            {
                throw new InvalidOperationException(
                    "Account is locked out.");
            }

            if (result.IsNotAllowed)
            {
                throw new InvalidOperationException(
                    "Sign-in is not allowed.");
            }

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    "UserName or Password invalid.");
            }
        }





        public async Task<int> RegisterAsync(RegisterDto input)
        {
            ArgumentNullException.ThrowIfNull(input);

            if (input.Education is null)
                throw new InvalidOperationException("Education is required.");

            if (input.Gender is null)
                throw new InvalidOperationException("Gender is required.");

            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var user = new User
                {
                    Name = input.Name,
                    Family = input.Family,
                    FatherName = input.FatherName,
                    DateOfBirth = input.DateOfBirth,
                    Education = input.Education.Value,
                    Gender = input.Gender.Value,
                    Address = input.Address,
                    UserName = input.UserName,
                    PhoneNumber = input.PhoneNumber,
                    Email = input.Email
                };

                var createResult =
                    await _userManager.CreateAsync(user, input.Password);

                if (!createResult.Succeeded)
                {
                    throw new RegistrationException(
                        GetIdentityErrors(createResult));
                }

                _walletService.CreateForUser(user.Id);

                var roleResult =
                    await _userManager.AddToRoleAsync(user, Roles.User);

                if (!roleResult.Succeeded)
                {
                    throw new RegistrationException(
                        GetIdentityErrors(roleResult));
                }

                await transaction.CommitAsync();

                return user.Id;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private List<string> GetIdentityErrors(IdentityResult result)
        {
            return result.Errors
                .Select(error => _identityErrorLocalizer.Localizer(error.Code))
                .ToList();
        }
    }
}
