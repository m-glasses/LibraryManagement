using LibraryManagement.Models;

namespace LibraryManagement.Services.Interfaces
{
    public interface ILoanService : IGenericService<Loan>
    {
        Loan Borrow(int userId, int bookCopyId);
        bool RequestReturn(int loanId);
        bool ConfirmReturn(int loanId, DateTime returnDate);
        bool Renew(int loanId);
    }
}
