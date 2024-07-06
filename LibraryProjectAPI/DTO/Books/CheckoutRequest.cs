using System.Text.Json.Serialization;

namespace LibraryProjectAPI.DTO.Books
{
    // Not used
    public class CheckoutRequest
    {
        [JsonPropertyName("isbn")]
        public int Isbn { get; set; }
        [JsonPropertyName("checkout")]
        public bool Checkout {  get; set; }
    }
}
