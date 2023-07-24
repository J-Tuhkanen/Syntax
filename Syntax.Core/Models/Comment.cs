using Syntax.Core.Models.Base;
using System;

namespace Syntax.Core.Models
{
    public class Comment : EntityWithId, IUserActivity
    {
        // Showcase of pull request
        public string PostId { get; set; }
        public string UserId { get; set; }
        public string Content { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
