using Syntax.Core.Models.Base;
using System;
using System.ComponentModel.DataAnnotations;

namespace Syntax.Core.Models
{
    public class Post : EntityWithId, IUserActivity
    {
        public string UserId { get; set; }

        [MaxLength(80)]
        public string Title { get; set; }
        public string Body { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
