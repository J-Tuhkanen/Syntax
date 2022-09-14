using Syntax.Models.Base;
using System;

namespace Syntax.Models
{
    public class Comment : EntityWithId, IUserActivity
    {
        public string PostId { get; set; }
        public string UserId { get; set; }
        public string Content { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
