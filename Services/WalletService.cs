using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.Services.Interfaces;

namespace LibraryManagement.Services
{
    public class WalletService : GenericService<Wallet>, IWalletService
    {
        private readonly LibraryDbContext _context;
        public WalletService(LibraryDbContext context) : base(context)
        {
            context = _context;
        }
    }
}
