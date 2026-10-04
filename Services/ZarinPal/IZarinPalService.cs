namespace LibraryManagement.Services.ZarinPal
{
    public interface IZarinPalService
    {
        Task<PaymentRequestResult> RequestPaymentAsync(
            decimal amountInTomans,
            string callbackUrl);

        Task<PaymentVerifyResult> VerifyPaymentAsync(
            string authority,
            decimal amount);

        string GetPaymentUrl(string authority);
    }
}

