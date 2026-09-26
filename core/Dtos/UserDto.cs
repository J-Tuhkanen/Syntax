using Syntax.Core.Models;
using System.Text.Json.Serialization;

namespace Syntax.Core.Dtos
{
    public class UserDto
    {
        [JsonConstructor]
        public UserDto()
        {
        }

        public UserDto(UserAccount account)
        {
            DisplayName = account.UserName;
            Email = account.Email;
            JoinedDate = account.JoinedDate;
        }

        public string DisplayName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime JoinedDate { get; set; }
    }
}
