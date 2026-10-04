using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace LibraryManagement.Services
{
    public class WalletService : GenericService<Wallet>, IWalletService
    {
        private readonly LibraryDbContext _context;
        public WalletService(LibraryDbContext context) : base(context)
        {
            _context = context;
        }

        public Wallet CreateForUser(int userId)
        {
            var userExists = _context.Users
                .Any(user => user.Id == userId);

            if (!userExists)
            {
                throw new InvalidOperationException("User not found.");
            }

            var walletExists = _context.Wallets
                .Any(wallet => wallet.UserId == userId);

            if (walletExists)
            {
                throw new InvalidOperationException(
                    "User already has a wallet.");
            }

            var wallet = new Wallet
            {
                UserId = userId,
                Balance = 0
            };

            _context.Wallets.Add(wallet);
            _context.SaveChanges();

            return wallet;
        }

        public void Deposit(int walletId, decimal amount)
        {
            if (amount <= 0)
            {
                throw new InvalidOperationException("Amount must be greater than zero.");
            }

            var wallet = _context.Wallets.FirstOrDefault(wallet => wallet.Id == walletId);

            if (wallet is null)
            {
                throw new InvalidOperationException("Wallet not found.");
            }

            wallet.Balance += amount;

            var walletTransaction = new WalletTransaction
            {
                WalletId = walletId,
                Amount = amount,
                Type = TransactionType.Deposit,
                CreatedAt = DateTime.Now
            };

            _context.WalletTransactions.Add(walletTransaction);

            _context.SaveChanges();
        }

        public Wallet? GetByUserId(int userId)
        {
            return _context.Wallets
                .FirstOrDefault(w => w.UserId == userId);
        }

        public void Withdraw(int walletId, decimal amount)
        {
            if (amount <= 0)
            {
                throw new InvalidOperationException("Amount must be greater than zero.");
            }

            var wallet = _context.Wallets.FirstOrDefault(wallet => wallet.Id == walletId);

            if (wallet is null)
            {
                throw new InvalidOperationException("Wallet not found.");
            }

            wallet.Balance -= amount;

            var walletTransaction = new WalletTransaction
            {
                WalletId = walletId,
                Amount = amount,
                Type = TransactionType.Withdrawal,
                CreatedAt = DateTime.Now
            };

            _context.WalletTransactions.Add(walletTransaction);

            _context.SaveChanges();
        }

        public List<WalletTransaction> GetTransactions(int walletId)
        {
            return _context.WalletTransactions
                    .AsNoTracking()
                    .Where(transaction => transaction.WalletId == walletId)
                    .OrderByDescending(transaction => transaction.CreatedAt)
                    .ToList();
        }
    }
}
