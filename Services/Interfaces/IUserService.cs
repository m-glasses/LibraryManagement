using LibraryManagement.DTO;
using LibraryManagement.Models;

namespace LibraryManagement.Services.Interfaces
{
    public interface IUserService
    {
        UserListResult GetAllUsers(UserListQueryDto input);
        UserDetailsDto? GetUserDetails(int id);
    }
}
