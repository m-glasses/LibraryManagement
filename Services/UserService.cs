using LibraryManagement.Data;
using LibraryManagement.DTO;
using LibraryManagement.Models;
using LibraryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;

namespace LibraryManagement.Services
{
    public class UserService : IUserService
    {
        private readonly LibraryDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IWalletService _wallet;

        public UserService(LibraryDbContext context , UserManager<User> userManager , IWalletService wallet)
        {
            _context = context;
            _userManager = userManager;
            _wallet = wallet;
        }

        public void Activate(int userId)
        {
            var user = _context.Users
                .FirstOrDefault(user => user.Id == userId);

            if (user is null)
            {
                throw new InvalidOperationException("User not found.");
            }

            if (user.IsActive)
            {
                throw new InvalidOperationException("User is already active.");
            }

            user.IsActive = true;

            _context.SaveChanges();
        }


        public async Task<int> CreateUserAsync(AdminUserCreateDto input )
        {
            if (input is null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (input.Role != Roles.User && input.Role != Roles.Admin)
            {
                throw new InvalidOperationException("Invalid role.");
            }

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
                    Education = input.Education
                        ?? throw new InvalidOperationException("Education is required."),
                    Gender = input.Gender
                        ?? throw new InvalidOperationException("Gender is required."),
                    Address = input.Address,
                    UserName = input.UserName,
                    PhoneNumber = input.PhoneNumber,
                    Email = input.Email
                };

                var createResult =
                    await _userManager.CreateAsync(user, input.Password);

                CheckIdentityResult(createResult);

                var roleResult =
                    await _userManager.AddToRoleAsync(user, input.Role);

                CheckIdentityResult(roleResult);

                _wallet.CreateForUser(user.Id);

                await transaction.CommitAsync();

                return user.Id;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        public async Task Deactivate(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null)
            {
                throw new InvalidOperationException("User not found.");
            }

            if (!user.IsActive)
            {
                throw new InvalidOperationException("User is already inactive.");
            }

            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains(Roles.Admin))
            {
                throw new InvalidOperationException(
                    "You cannot deactivate an admin.");
            }

            var hasOpenLoan = await _context.Loans.AnyAsync(loan =>
                loan.UserId == userId &&
                (loan.LoanStatus == LoanStatus.Active ||
                 loan.LoanStatus == LoanStatus.ReturnPending));

            if (hasOpenLoan)
            {
                throw new InvalidOperationException(
                    "First close all open loans.");
            }

            var hasActiveReservation = await _context.Reservations.AnyAsync(reservation =>
                reservation.UserId == userId &&
                reservation.ReservationStatus == ReservationStatus.Active);

            if (hasActiveReservation)
            {
                throw new InvalidOperationException(
                    "First close all active reservations.");
            }

            user.IsActive = false;

            await _context.SaveChangesAsync();
        }



        public UserListResult GetAllUsers(UserListQueryDto input)
        {
            var usersQuery = _context.Users
                .Join(
                    _context.UserRoles,
                    user => user.Id,
                    userRole => userRole.UserId,
                    (user, userRole) => new
                    {
                        User = user,
                        UserRole = userRole
                    })
                .Join(
                    _context.Roles,
                    userRoleData => userRoleData.UserRole.RoleId,
                    role => role.Id,
                    (userRoleData, role) => new
                    {
                        User = userRoleData.User,
                        Role = role.Name
                    });

            if (!string.IsNullOrWhiteSpace(input.SearchTerm))
            {
                usersQuery = usersQuery.Where(user =>
                    user.User.Name.Contains(input.SearchTerm)
                    || user.User.Family.Contains(input.SearchTerm)
                    || (user.User.Name + " " + user.User.Family).Contains(input.SearchTerm)
                    || user.User.Email.Contains(input.SearchTerm)
                    || user.User.PhoneNumber.Contains(input.SearchTerm));
            }

            if (!string.IsNullOrWhiteSpace(input.Role))
            {
                usersQuery = usersQuery.Where(user =>
                    user.Role == input.Role);
            }

            if (input.IsDebtor is not null)
            {
                if (input.IsDebtor.Value)
                {
                    usersQuery = usersQuery.Where(user =>
                        user.User.Wallet.Balance < 0);
                }
                else
                {
                    usersQuery = usersQuery.Where(user =>
                        user.User.Wallet.Balance >= 0);
                }
            }

            var totalCount = usersQuery.Count();

            var skip = (input.Page - 1) * input.PageSize;

            var users = usersQuery
                .OrderBy(user => user.User.Id)
                .Skip(skip)
                .Take(input.PageSize)
                .Select(user => new UserListDto
                {
                    Id = user.User.Id,
                    Name = user.User.Name,
                    Family = user.User.Family,
                    Email = user.User.Email,
                    PhoneNumber = user.User.PhoneNumber,
                    Role = user.Role,
                    WalletBalance = user.User.Wallet.Balance
                })
                .ToList();

            return new UserListResult
            {
                Users = users,
                CurrentPage = input.Page,
                PageSize = input.PageSize,
                TotalCount = totalCount
            };
        }

        public UserDetailsDto? GetUserDetails(int id)
        {
            var user = _context.Users
                .Join(
                    _context.UserRoles,
                    user => user.Id,
                    userRole => userRole.UserId,
                    (user, userRole) => new
                    {
                        User = user,
                        UserRole = userRole
                    })
                .Join(
                    _context.Roles,
                    userRoleData => userRoleData.UserRole.RoleId,
                    role => role.Id,
                    (userRoleData, role) => new UserDetailsDto
                    {
                        Id = userRoleData.User.Id,
                        Name = userRoleData.User.Name,
                        Family = userRoleData.User.Family,
                        FatherName = userRoleData.User.FatherName,
                        UserName = userRoleData.User.UserName,
                        Email = userRoleData.User.Email,
                        DateOfBirth = userRoleData.User.DateOfBirth,
                        Gender = userRoleData.User.Gender,
                        Education = userRoleData.User.Education,
                        PhoneNumber = userRoleData.User.PhoneNumber,
                        Role = role.Name,
                        Address = userRoleData.User.Address,
                        WalletBalance = userRoleData.User.Wallet.Balance,
                        IsActive = userRoleData.User.IsActive
                    })
                .FirstOrDefault(user => user.Id == id);

            return user;
        }

        public async Task UpdateUserAsync(AdminUserUpdateDto input , int currentUserId)
        {
            if (input is null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (input.Role != Roles.User && input.Role != Roles.Admin)
            {
                throw new InvalidOperationException("Invalid role.");
            }

            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var user =
                    await _userManager.FindByIdAsync(input.Id.ToString());

                if (user is null)
                {
                    throw new InvalidOperationException("User not found.");
                }

                var roles = await _userManager.GetRolesAsync(user);
                var currentRole = roles.FirstOrDefault();

                if (currentRole == Roles.Admin)
                {
                    if (user.Id != currentUserId)
                    {
                        throw new InvalidOperationException(
                            "Admin users cannot be edited.");
                    }

                    if (input.Role == Roles.User)
                    {
                        throw new InvalidOperationException(
                            "Admin cannot change their role to User.");
                    }
                }

                user.Name = input.Name;
                user.Family = input.Family;
                user.FatherName = input.FatherName;
                user.DateOfBirth = input.DateOfBirth;
                user.Education = input.Education
                    ?? throw new InvalidOperationException(
                        "Education is required.");
                user.Gender = input.Gender
                    ?? throw new InvalidOperationException(
                        "Gender is required.");
                user.Address = input.Address;

                var result = await _userManager.UpdateAsync(user);
                CheckIdentityResult(result);

                result = await _userManager.SetUserNameAsync(
                    user,
                    input.UserName);
                CheckIdentityResult(result);

                result = await _userManager.SetEmailAsync(
                    user,
                    input.Email);
                CheckIdentityResult(result);

                result = await _userManager.SetPhoneNumberAsync(
                    user,
                    input.PhoneNumber);
                CheckIdentityResult(result);

                if (currentRole != input.Role)
                {
                    if (currentRole is not null)
                    {
                        result = await _userManager.RemoveFromRoleAsync(
                            user,
                            currentRole);
                        CheckIdentityResult(result);
                    }

                    result = await _userManager.AddToRoleAsync(
                        user,
                        input.Role);
                    CheckIdentityResult(result);
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private void CheckIdentityResult(IdentityResult result)
        {
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    string.Join(", ", result.Errors.Select(error => error.Description)));
            }
        }
    }
}
