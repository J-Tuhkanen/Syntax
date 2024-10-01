using Syntax.Core.Models.Base;

namespace Syntax.Core.Models
{
    public class Comment : EntityBase
    {
        public Comment()
        {
            Timestamp = DateTime.UtcNow;
        }

        public Topic? Topic { get; set; } = null!;
        public UserAccount User { get; set; } = null!;
        public string Content { get; set; } = string.Empty;
    }
}
