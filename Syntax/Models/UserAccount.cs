using Microsoft.AspNetCore.Identity;

namespace Syntax.Models
{
    public class UserAccount : IdentityUser
    {
        public bool IsDeleted { get; set; }
    }
}
