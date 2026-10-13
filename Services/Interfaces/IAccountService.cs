using LibraryManagement.DTO;
using LibraryManagement.Models;
using Microsoft.AspNetCore.Identity;

namespace LibraryManagement.Services.Interfaces
{
    public interface IAccountService
    {
        Task<int> RegisterAsync(RegisterDto input);
        Task LoginAsync(LoginDto input);
    }
}
