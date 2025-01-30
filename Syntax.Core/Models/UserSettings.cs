using Syntax.Core.Models.Base;

namespace Syntax.Core.Models
{
    public class UserSettings : EntityBase
    {
        public string DisplayName { get; set; } = null!;
        public bool ShowTopics { get; set; }
        public bool ShowComments { get; set; } 
        public string? ProfilePicture { get; set; }
    }
}
