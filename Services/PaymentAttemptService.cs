using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Services
{
    public class PaymentAttemptService : GenericService<PaymentAttempt> , IPaymentAttemptService 
    {
        private readonly LibraryDbContext _context;
        private readonly IWalletService _walletService;
        public PaymentAttemptService(LibraryDbContext context , IWalletService wallet) : base(context)
        {
            _context = context;
            _walletService = wallet;
        }
        public PaymentAttempt Create(int userId, decimal amount)
        {
            if (userId <= 0)
                throw new ArgumentException("User ID must be greater than zero.", nameof(userId));

            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

            var userExists = _context.Users.Any(user => user.Id == userId);

            if (!userExists)
                throw new InvalidOperationException("User not found.");

            var paymentAttempt = new PaymentAttempt
            {
                UserId = userId,
                Amount = amount,
                Status = PaymentStatus.Pending,
                CreatedAt = DateTime.Now
            };

            _context.PaymentAttempts.Add(paymentAttempt);
            _context.SaveChanges();

            return paymentAttempt;
        }

        public PaymentAttempt? GetByAuthority(string authority)
        {
            if (string.IsNullOrWhiteSpace(authority))
                throw new ArgumentException("Authority cannot be empty.", nameof(authority));

            return _context.PaymentAttempts
                .FirstOrDefault(payment => payment.Authority == authority);
        }

        public void SetAuthority(PaymentAttempt paymentAttempt, string authority)
        {
            ArgumentNullException.ThrowIfNull(paymentAttempt);

            if (string.IsNullOrWhiteSpace(authority))
                throw new ArgumentException("Authority cannot be empty",nameof(authority));

            paymentAttempt.Authority = authority;

            _context.SaveChanges();
        }

        public void SetStatus(PaymentAttempt paymentAttempt, PaymentStatus status)
        {
            ArgumentNullException.ThrowIfNull(paymentAttempt);

            if (!Enum.IsDefined(status))
                throw new ArgumentException("Invalid payment status",nameof(status));

            if (paymentAttempt.Status != PaymentStatus.Pending)
                throw new InvalidOperationException("Only pending payment attempts can change status");

            paymentAttempt.Status = status;

            _context.SaveChanges();
        }

        public void CompletePayment(PaymentAttempt paymentAttempt)
        {
            ArgumentNullException.ThrowIfNull(paymentAttempt);

            if (paymentAttempt.Status != PaymentStatus.Pending)
                throw new InvalidOperationException("Only pending payment attempts can be completed.");

            var wallet = _walletService.GetByUserId(paymentAttempt.UserId);

            if (wallet is null)
                throw new InvalidOperationException("Wallet not found.");

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                _walletService.Deposit(wallet.Id,paymentAttempt.Amount);

                paymentAttempt.Status = PaymentStatus.Verified;

                _context.SaveChanges();

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
