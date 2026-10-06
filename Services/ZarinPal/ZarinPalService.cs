using Azure.Core;
using LibraryManagement.Configuration;
using LibraryManagement.DTO;
using LibraryManagement.Models;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace LibraryManagement.Services.ZarinPal
{
    public class ZarinpalService : IZarinPalService
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<ZarinPalOptions> _zarinPalOption;
        public ZarinpalService(HttpClient client , IOptions<ZarinPalOptions> options)
        {
            _httpClient = client;
            _zarinPalOption = options;
        }

        public async Task<PaymentRequestResult> RequestPaymentAsync(decimal amountInTomans , string callbackUrl , User user)
        {
 
            var request = new PaymentRequest()
            {
                Amount = (long)amountInTomans * 10,
                MerchantId = _zarinPalOption.Value.MerchantId,
                CallbackUrl = callbackUrl,
                Description = "شارژ کیف‌پول",
                Metadata = new List<Metadata>
                {
                    new Metadata
                    {
                        Mobile = user.PhoneNumber,
                        Email = user.Email
                    }
                }
            };
   
            var response = await _httpClient.PostAsJsonAsync(_zarinPalOption.Value.RequestUrl,request);

            response.EnsureSuccessStatusCode();

            string result = await response.Content.ReadAsStringAsync();

            var responseData = JsonSerializer.Deserialize<ZarinPalResponse<PaymentRequestResult>>(result);

            if (responseData?.Data is null)
            {
                throw new InvalidOperationException("Invalid response received from ZarinPal");
            }

            if (responseData.Data.Code != 100)
            {
                throw new InvalidOperationException($"ZarinPal payment request failed. Code: {responseData.Data.Code}, Message: {responseData.Data.Message}");
            }

            return responseData.Data;
        }

        public async Task<PaymentVerifyResult> VerifyPaymentAsync(string authority, decimal amountInTomans)
        {
            var verifyRequest = new PaymentVerifyRequest()
            {
                Amount = (long)amountInTomans * 10,
                Authority = authority,
                MerchantId = _zarinPalOption.Value.MerchantId
            };

            var response = await _httpClient.PostAsJsonAsync(_zarinPalOption.Value.VerifyUrl, verifyRequest);
            response.EnsureSuccessStatusCode();


            string result = await response.Content.ReadAsStringAsync();

            var responseData = JsonSerializer.Deserialize<ZarinPalResponse<PaymentVerifyResult>>(result);

            if (responseData?.Data is null)
            {
                throw new InvalidOperationException(
                    "Invalid response received from ZarinPal.");
            }

            if (responseData.Data.Code != 100 && responseData.Data.Code != 101)
            {
                throw new InvalidOperationException($"ZarinPal payment verification failed. " +$"Code: {responseData.Data.Code}, " +$"Message: {responseData.Data.Message}");
            }

            return responseData.Data;
        }
       
        public string GetPaymentUrl(string authority)
        {
            if (string.IsNullOrWhiteSpace(authority))
            {
                throw new InvalidOperationException("Payment authority is invalid.");
            }

            return $"{_zarinPalOption.Value.StartPayUrl}{authority}";
        }


    }
}
