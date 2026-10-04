using LibraryManagement.Models;

namespace LibraryManagement.Services.ZarinPal
{
    public interface IZarinPalService
    {
        Task<PaymentRequestResult> RequestPaymentAsync(
            decimal amountInTomans,
            string callbackUrl,
            User user);

        Task<PaymentVerifyResult> VerifyPaymentAsync(
            string authority,
            decimal amount);

        string GetPaymentUrl(string authority);
    }
}

