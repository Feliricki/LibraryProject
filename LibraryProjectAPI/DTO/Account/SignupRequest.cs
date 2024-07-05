using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LibraryProjectAPI.DTO.Account
{
    public class SignupRequest
    {
        [Required(ErrorMessage = "UserName is required")]
        [MaxLength(255)]
        [JsonPropertyName("username")]
        public string UserName { get; set; } = null!;

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        [MaxLength(255)]
        [JsonPropertyName("email")]
        public string Email { get; set; } = null!;
        [MaxLength(500)]
        [Required(ErrorMessage = "Password is required")]
        [JsonPropertyName("password")]
        public string Password { get; set; } = null!;
        [Required(ErrorMessage = "isLibrarian field needs to be filled out")]
        [JsonPropertyName("isLibrarian")]
        public bool IsLibrarian { get; set; }
    }
}
