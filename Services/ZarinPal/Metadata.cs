using System.Text.Json.Serialization;

namespace LibraryManagement.Services.ZarinPal
{
    public class Metadata
    {
        [JsonPropertyName("mobile")]
        public string? Mobile {  get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }
    }
}
