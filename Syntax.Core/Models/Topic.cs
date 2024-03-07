using Syntax.Core.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace Syntax.Core.Models
{
    public class Topic : EntityBase, IUserActivity
    {
        public Topic() 
        {
            Timestamp = DateTime.UtcNow;
        }
        
        public virtual UserAccount User { get; set; }

        [MaxLength(80)]
        public string Title { get; set; }
        
        public string Body { get; set; }
    }
}
