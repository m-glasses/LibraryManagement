using LibraryManagement.Models;
using System.Net;

namespace LibraryManagement.Services.Interfaces
{
    public interface ILoanService : IGenericService<Loan>
    {
        BookCopy FindAvailableBookCopy(int bookId);
        Loan Borrow(int userId, int bookCopyId);
        bool RequestReturn(int loanId);
        bool ConfirmReturn(int loanId, DateTime returnDate);
        bool Renew(int loanId);
    }
}
