using System.Text.Json.Serialization;

namespace LibraryManagement.DTO
{
    public class PaymentVerifyRequest
    {
        [JsonPropertyName("merchant_id")]
        public string MerchantId { get; set; }

        [JsonPropertyName("authority")]
        public string Authority { get; set; }

        [JsonPropertyName("amount")]
        public long Amount { get; set; }

    }
}
