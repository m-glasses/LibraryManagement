using LibraryManagement.Models;
using System.Net;

namespace LibraryManagement.Services.Interfaces
{
    public interface ILoanService : IGenericService<Loan>
    {
        BookCopy CanBorrowBook(int userId, int bookId);
        Loan Borrow(int userId, int bookId);
        bool RequestReturn(int loanId , int userId );
        bool ConfirmReturn(int loanId, DateTime returnDate);
        bool Renew(int loanId);
        Loan? GetDetailsById(int loanId, int userId);
        List<Loan> GetUserLoans(int userId);
    }
}
