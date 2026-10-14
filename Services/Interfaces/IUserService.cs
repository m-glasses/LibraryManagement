using LibraryManagement.DTO.User;
using LibraryManagement.Models;

namespace LibraryManagement.Services.Interfaces
{
    public interface IUserService
    {
        UserListResult GetAllUsers(UserListQueryDto input);
        UserDetailsDto? GetUserDetails(int id);
        Task Deactivate(int userId);
        void Activate(int userId);
        Task UpdateUserAsync(AdminUserUpdateDto input, int currentUserId);
        Task<int> CreateUserAsync(AdminUserCreateDto input);
    }
}
