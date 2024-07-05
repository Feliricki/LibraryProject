using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LibraryProjectAPI.DTO.Account
{
    public class LoginRequest
    {
        // The username can be either an email or a username
        [Required(ErrorMessage = "Username is required")]
        [MaxLength(255)]
        [JsonPropertyName("username")]
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [MaxLength(255)]
        [JsonPropertyName("password")]
        public string Password { get; set; } = null!;

        //[Required(ErrorMessage = "User must be a librarian or a normal user.")]
        //[JsonPropertyName("isLibrarian")]
        // public bool IsLibrarian { get; set; } = false;
    }
}
