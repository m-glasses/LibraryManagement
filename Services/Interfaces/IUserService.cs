using LibraryManagement.DTO;
using LibraryManagement.Models;

namespace LibraryManagement.Services.Interfaces
{
    public interface IUserService
    {
        List<UserListDto> GetAllUsers();
        UserDetailsDto? GetUserDetails(int id);
    }
}
