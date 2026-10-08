using LibraryManagement.Data;
using LibraryManagement.DTO;
using LibraryManagement.Models;
using LibraryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Services
{
    public class UserService : IUserService
    {
        private readonly LibraryDbContext _context;
        private readonly UserManager<User> _userManager;

        public UserService(LibraryDbContext context , UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
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


        public void Deactivate(int userId)
        {
            var user = _context.Users
                .FirstOrDefault(user => user.Id == userId);

            if (user is null)
            {
                throw new InvalidOperationException("User not found.");
            }

            if (!user.IsActive)
            {
                throw new InvalidOperationException("User is already inactive.");
            }

            var hasOpenLoan = _context.Loans
                .Any(loan =>
                    loan.UserId == userId &&
                    (loan.LoanStatus == LoanStatus.Active ||
                     loan.LoanStatus == LoanStatus.ReturnPending));

            if (hasOpenLoan)
            {
                throw new InvalidOperationException(
                    "First close all open loans.");
            }

            var hasActiveReservation = _context.Reservations
                .Any(reservation =>
                    reservation.UserId == userId &&
                    reservation.ReservationStatus == ReservationStatus.Active);

            if (hasActiveReservation)
            {
                throw new InvalidOperationException(
                    "First close all active reservations.");
            }

            user.IsActive = false;

            _context.SaveChanges();
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

        public async Task UpdateUserAsync(AdminUserUpdateDto input)
        {
            if (input is null)
            {
                throw new ArgumentNullException(nameof(input));
            }


            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
 
                var user = await _userManager.FindByIdAsync(input.Id.ToString());

                if (user is null)
                {
                    throw new InvalidOperationException("User not found.");
                }

                user.Name = input.Name;
                user.Family = input.Family;
                user.FatherName = input.FatherName;
                user.DateOfBirth = input.DateOfBirth;
                user.Education = input.Education
                    ?? throw new InvalidOperationException("Education is required.");

                user.Gender = input.Gender
                    ?? throw new InvalidOperationException("Gender is required.");

                user.Address = input.Address;

                var result = await _userManager.UpdateAsync(user);

                CheckIdentityResult(result);

                result = await _userManager.SetUserNameAsync(user, input.UserName);

                CheckIdentityResult(result);

                result = await _userManager.SetEmailAsync(user, input.Email);

                CheckIdentityResult(result);

                result = await _userManager.SetPhoneNumberAsync(user, input.PhoneNumber);

                CheckIdentityResult(result);

                var roles = await _userManager.GetRolesAsync(user);
                var currentRole = roles.FirstOrDefault();

                if (currentRole != input.Role)
                {
                    if (currentRole is not null)
                    {
                        result = await _userManager.RemoveFromRoleAsync(user, currentRole);

                        CheckIdentityResult(result);
                    }

                    result = await _userManager.AddToRoleAsync(user, input.Role);

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
