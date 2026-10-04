using System.Text.Json.Serialization;

namespace LibraryManagement.Services.ZarinPal
{
    public class ZarinPalResponse<T>
    {
        [JsonPropertyName("data")]
        public T? Data { get; set; }
    }
}