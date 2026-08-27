using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.Services.Interfaces;

namespace LibraryManagement.Services
{
    public class UserService : IUserService , GenericService<User>
    {
        private readonly LibraryDbContext _context;
        public UserService(LibraryDbContext context) : base(context) 
        {
            _context = context;
        }
       
    }
}
