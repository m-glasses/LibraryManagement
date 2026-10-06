using System.Text.Json.Serialization;

namespace LibraryManagement.DTO
{
    public class Metadata
    {
        [JsonPropertyName("mobile")]
        public string? Mobile {  get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }
    }
}
