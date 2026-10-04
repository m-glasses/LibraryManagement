using LibraryManagement.Models;

namespace LibraryManagement.Services.Interfaces
{
    public interface IPaymentAttemptService : IGenericService<PaymentAttempt>
    {
        PaymentAttempt Create(int userId, decimal amount);

        PaymentAttempt? GetByAuthority(string authority);

        void SetAuthority(PaymentAttempt paymentAttempt, string authority);

        void SetStatus(PaymentAttempt paymentAttempt, PaymentStatus status);
        void CompletePayment(PaymentAttempt paymentAttempt);
    }
}