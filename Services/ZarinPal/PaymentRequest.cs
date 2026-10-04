using System.Text.Json.Serialization;

namespace LibraryManagement.Services.ZarinPal
{
    public class PaymentRequest
    {
        [JsonPropertyName("merchant_id")]
        public string MerchantId { get; set; }

        [JsonPropertyName("amount")]
        public long Amount { get; set; }

        [JsonPropertyName("callback_url")]
        public string CallbackUrl { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("metadata")]
        public List<Metadata>? Metadata { get; set; }
    }
}
