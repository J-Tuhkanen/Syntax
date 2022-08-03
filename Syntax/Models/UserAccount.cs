using Microsoft.AspNetCore.Identity;
using System;

namespace Syntax.Models
{
    public class UserAccount : IdentityUser
    {
        public bool IsDeleted { get; set; }

        public string ProfilePictureFileId { get; set; }
    
        public DateTime JoinedDate { get; set; }
    }
}
