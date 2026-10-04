using System.Text.Json.Serialization;

namespace LibraryManagement.Services.ZarinPal
{
    public class PaymentRequestResult
    {
        [JsonPropertyName("authority")]
        public string? Authority { get; set; }

        [JsonPropertyName("fee")]
        public long? Fee { get; set; }

        [JsonPropertyName("fee_type")]
        public string? FeeType { get; set; }

        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }
}