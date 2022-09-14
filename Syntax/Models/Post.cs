using Syntax.Models.Base;
using System;

namespace Syntax.Models
{
    public class Post : EntityWithId, IUserActivity
    {
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
