using LibraryManagement.Data;
using LibraryManagement.DTO.User;
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
            ArgumentNullException.ThrowIfNull(input);

            if (input.Page < 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(input.Page),
                    "Page must be greater than zero.");
            }

            if (input.PageSize < 1 || input.PageSize > 50)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(input.PageSize),
                    "PageSize must be between 1 and 50.");
            }

            var usersQuery = _context.Users
                .AsNoTracking()
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

            // 1. Search
            if (!string.IsNullOrWhiteSpace(input.SearchTerm))
            {
                var searchTerm = input.SearchTerm.Trim();

                usersQuery = usersQuery.Where(user =>
                    user.User.Name.Contains(searchTerm) ||
                    user.User.Family.Contains(searchTerm) ||
                    (user.User.Name + " " + user.User.Family)
                        .Contains(searchTerm) ||
                    (user.User.Email != null &&
                        user.User.Email.Contains(searchTerm)) ||
                    (user.User.PhoneNumber != null &&
                        user.User.PhoneNumber.Contains(searchTerm)));
            }

            // 2. Filter by role
            if (!string.IsNullOrWhiteSpace(input.Role))
            {
                usersQuery = usersQuery.Where(user =>
                    user.Role == input.Role);
            }

            // 3. Filter by debtor status
            if (input.IsDebtor.HasValue)
            {
                usersQuery = input.IsDebtor.Value
                    ? usersQuery.Where(user =>
                        user.User.Wallet.Balance < 0)
                    : usersQuery.Where(user =>
                        user.User.Wallet.Balance >= 0);
            }

            // 4. Count filtered results before pagination
            var totalCount = usersQuery.Count();

            var totalPages = (int)Math.Ceiling(
                totalCount / (double)input.PageSize);

            var skip = (int)Math.Min(
                ((long)input.Page - 1) * input.PageSize,
                int.MaxValue);

            // 5. Sorting
            usersQuery = input.SortBy switch
            {
                "Name" => input.SortDescending
                    ? usersQuery.OrderByDescending(user => user.User.Name)
                        .ThenBy(user => user.User.Id)
                    : usersQuery.OrderBy(user => user.User.Name)
                        .ThenBy(user => user.User.Id),

                "Family" => input.SortDescending
                    ? usersQuery.OrderByDescending(user => user.User.Family)
                        .ThenBy(user => user.User.Id)
                    : usersQuery.OrderBy(user => user.User.Family)
                        .ThenBy(user => user.User.Id),

                "Email" => input.SortDescending
                    ? usersQuery.OrderByDescending(user => user.User.Email)
                        .ThenBy(user => user.User.Id)
                    : usersQuery.OrderBy(user => user.User.Email)
                        .ThenBy(user => user.User.Id),

                "WalletBalance" => input.SortDescending
                    ? usersQuery.OrderByDescending(user =>
                            user.User.Wallet.Balance)
                        .ThenBy(user => user.User.Id)
                    : usersQuery.OrderBy(user =>
                            user.User.Wallet.Balance)
                        .ThenBy(user => user.User.Id),

                _ => input.SortDescending
                    ? usersQuery.OrderByDescending(user => user.User.Id)
                    : usersQuery.OrderBy(user => user.User.Id)
            };

            // 6. Pagination and projection
            var users = usersQuery
                .Skip(skip)
                .Take(input.PageSize)
                .Select(user => new UserListDto
                {
                    Id = user.User.Id,
                    Name = user.User.Name,
                    Family = user.User.Family,
                    Email = user.User.Email!,
                    PhoneNumber = user.User.PhoneNumber!,
                    Role = user.Role!,
                    WalletBalance = user.User.Wallet.Balance
                })
                .ToList();

            return new UserListResult
            {
                Users = users,
                CurrentPage = input.Page,
                PageSize = input.PageSize,
                TotalCount = totalCount,
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
