using LibraryManagement.Models;

namespace LibraryManagement.Services.Interfaces
{
    public interface ILoanService : IGenericService<Loan>
    {
        BookCopy GetAvailableBookCopy(int userId, int bookId);
        Loan Borrow(int userId, int bookId);
        void RequestReturn(int loanId, int userId);
        void ConfirmReturn(int loanId, DateTime returnDate);
        void Renew(int loanId, int userId);
        Loan? GetDetailsById(int loanId, int userId);
        List<Loan> GetUserLoans(int userId);
        List<Loan> GetAllLoans();
        Loan? GetAdminDetailsById(int loanId);
    }
}
