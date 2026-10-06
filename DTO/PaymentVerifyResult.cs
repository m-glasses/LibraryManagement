using System.Text.Json.Serialization;

namespace LibraryManagement.DTO
{
    public class PaymentVerifyResult
    {
        [JsonPropertyName("fee")]
        public long? Fee { get; set; }

        [JsonPropertyName("fee_type")]
        public string? FeeType { get; set; }

        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("ref_id")]
        public long? RefId { get; set; }

        [JsonPropertyName("card_pan")]
        public string? CardPan { get; set; }
    }
}