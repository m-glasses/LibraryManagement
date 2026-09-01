using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.Services.Interfaces;

namespace LibraryManagement.Services
{
    public class UserService :   IUserService
    {
        private readonly LibraryDbContext _context;
        public UserService(LibraryDbContext context) 
        {
            _context = context;
        }
       
    }
}
