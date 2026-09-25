using LibraryManagement.Models;

namespace LibraryManagement.Services.Interfaces
{
    public interface IWalletService : IGenericService<Wallet>
    {
        Wallet? GetByUserId(int  userId);
        void Deposit (int walletId , decimal amount);
        void Withdraw (int walletId , decimal amount);
    }
}
