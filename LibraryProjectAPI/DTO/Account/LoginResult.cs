using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LibraryProjectAPI.DTO.Account
{
    public class LoginResult
    {
        [Required]
        [JsonPropertyName("success")]
        public bool Success { get; set; }
        [Required]
        [JsonPropertyName("message")]
        public string Message { get; set; }  = null!;
        [JsonPropertyName("role")]
        public string? Role { get; set; } = null!;
        [JsonPropertyName("token")]
        public string? Token { get; set; } = null!;
    }
}
