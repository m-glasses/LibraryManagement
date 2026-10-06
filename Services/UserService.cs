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

        public List<UserListDto> GetAllUsers()
        {
            var users = _context.Users
                .Join(
                    _context.UserRoles,
                    user => user.Id,
                    userRole => userRole.UserId,
                    (user, userRole) => new
                    {
                        User = user,
                        UserRole = userRole
                    }
                )
                .Join(
                    _context.Roles,
                    userRoleData => userRoleData.UserRole.RoleId,
                    role => role.Id,
                    (userRoleData, role) => new UserListDto
                    {
                        Id = userRoleData.User.Id,
                        Name = userRoleData.User.Name,
                        Family = userRoleData.User.Family,
                        Email = userRoleData.User.Email,
                        PhoneNumber = userRoleData.User.PhoneNumber,
                        Role = role.Name,
                        WalletBalance = userRoleData.User.Wallet.Balance
                    }
                )
                .ToList();

            return users;
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
