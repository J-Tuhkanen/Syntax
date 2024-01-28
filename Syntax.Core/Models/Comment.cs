using Syntax.Core.Models.Base;
using System;

namespace Syntax.Core.Models
{
    public class Comment : EntityBase, IUserActivity
    {
        public Comment()
        {
            Timestamp = DateTime.UtcNow;
        }

        public Guid TopicId { get; set; }
        public virtual Topic Topic { get; set; }
        public virtual UserAccount User { get; set; }
        public string Content { get; set; }
    }
}
