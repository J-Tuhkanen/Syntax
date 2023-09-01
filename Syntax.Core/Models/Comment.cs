using Syntax.Core.Models.Base;
using System;

namespace Syntax.Core.Models
{
    public class Comment : EntityWithId, IUserActivity
    {
        public Comment(string postId, string content, string userId)
        {
            PostId = postId;
            Content = content;
            UserId = userId;
            Timestamp = DateTime.UtcNow;
        }

        // Showcase of pull request
        public string PostId { get; set; }

        public string UserId { get; set; }

        public virtual UserAccount User { get; set; }

        public string Content { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
