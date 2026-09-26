using Syntax.Core.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace Syntax.Core.Models
{
    public class Topic : ActivityBase
    {
        public UserAccount User { get; set; } = null!;

        [MaxLength(80)]
        public string Title { get; set; } = string.Empty;
        
        public string Body { get; set; } = string.Empty;

        public List<Comment> Comments { get; set; } = new();
    }
}
