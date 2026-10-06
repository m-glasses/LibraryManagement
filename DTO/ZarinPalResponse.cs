using System.Text.Json.Serialization;

namespace LibraryManagement.DTO
{
    public class ZarinPalResponse<T>
    {
        [JsonPropertyName("data")]
        public T? Data { get; set; }
    }
}