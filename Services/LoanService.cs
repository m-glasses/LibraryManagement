using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.Services.Interfaces;

namespace LibraryManagement.Services
{
    public class LoanService : GenericService<Loan> ,ILoanService
    {
        private readonly LibraryDbContext _context;
        public LoanService(LibraryDbContext context): base(context) 
        {
            _context = context;
        }

    }
}
