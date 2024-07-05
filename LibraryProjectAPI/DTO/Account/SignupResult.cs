using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LibraryProjectAPI.DTO.Account
{
    public class SignupResult
    {
        [JsonPropertyName("success")]
        [Required]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        [Required]
        public string Message { get; set; } = null!;

        [JsonPropertyName("role")]
        public string? Role { get; set; } = null!;

        [JsonPropertyName("token")]
        public string Token { get; set; } = null!;
    }
}
