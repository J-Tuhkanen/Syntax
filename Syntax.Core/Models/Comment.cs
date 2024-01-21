using Syntax.Core.Models.Base;
using System;

namespace Syntax.Core.Models
{
    public class Comment : EntityBase, IUserActivity
    {
        public Comment(string postId, string content, string userId)
        {
            PostId = postId;
            Content = content;
            UserId = userId;
            Timestamp = DateTime.UtcNow;
        }

        public string PostId { get; set; }
        public virtual UserAccount User { get; set; }
        public string Content { get; set; }
    }
}
