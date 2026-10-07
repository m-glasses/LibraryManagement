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
                        Email = userRoleData.User.Email,
                        DateOfBirth = userRoleData.User.DateOfBirth,
                        Gender = userRoleData.User.Gender,
                        Education = userRoleData.User.Education,
                        PhoneNumber = userRoleData.User.PhoneNumber,
                        Role = role.Name,
                        Address = userRoleData.User.Address,
                        WalletBalance = userRoleData.User.Wallet.Balance
                    })
                .FirstOrDefault(user => user.Id == id);

            return user;
        }
    }
}
