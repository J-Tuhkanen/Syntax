using Syntax.Core.Models.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Syntax.Core.Models
{
    public class Post : EntityBase, IUserActivity
    {
        public Post(string title, string body, string userId) 
        {
            Title = title;
            Body = body;
            UserId = userId;
            Timestamp = DateTime.UtcNow;
        }
        
        public virtual UserAccount User { get; set; }

        [MaxLength(80)]
        public string Title { get; set; }
        
        public string Body { get; set; }
    }
}
