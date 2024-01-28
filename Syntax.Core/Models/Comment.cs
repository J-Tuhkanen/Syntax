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

        public Guid PostId { get; set; }
        public virtual Post Post { get; set; }
        public virtual UserAccount User { get; set; }
        public string Content { get; set; }
    }
}
