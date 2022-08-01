using Microsoft.AspNetCore.Identity;

namespace Syntax.Models
{
    public class UserAccount : IdentityUser
    {
        public bool IsDeleted { get; set; }

        public string ProfilePictureFileId { get; set; }
    }
}
