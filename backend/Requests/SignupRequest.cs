using System.ComponentModel.DataAnnotations;

namespace Syntax.API.Requests
{
    public class SignupRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        public string DisplayName { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
