using System.ComponentModel.DataAnnotations;

namespace Syntax.API.Requests
{
    public class SigninRequest
    {
        [Required]
        public string Username { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}