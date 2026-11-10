using LibraryManagement.DTO.Book;
using LibraryManagement.DTO.Loans;
using LibraryManagement.Models;

namespace LibraryManagement.Services.Interfaces
{
    public interface ILoanService : IGenericService<Loan>
    {
        BookCopy GetAvailableBookCopy(int bookId);
        Loan Borrow(int userId, int bookId);
        void RequestReturn(int loanId, int userId);
        void ConfirmReturn(int loanId, DateTime returnDate);
        void Renew(int loanId, int userId);
        Loan? GetDetailsById(int loanId, int userId);
        UserLoanListResult UserGetLoansList(int userId, LoanFilterDto input);
        AdminLoanListResult AdminGetLoansList(LoanFilterDto input);
        Loan? GetAdminDetailsById(int loanId);
    }
}
