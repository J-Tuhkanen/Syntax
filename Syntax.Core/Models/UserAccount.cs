using Microsoft.AspNetCore.Identity;
using System;

namespace Syntax.Core.Models
{
    public class UserAccount : IdentityUser
    {
        public List<Topic> UserTopics { get; set; }
        public List<Comment> UserComments { get; set; }

        public bool IsDeleted { get; set; }

        public Blob? ProfilePictureBlob { get; set; }

        public DateTime JoinedDate { get; set; }
    }
}
