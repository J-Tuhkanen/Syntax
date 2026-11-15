using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Syntax.Core.Models
{
    public class UserAccount : IdentityUser
    {
        public List<Topic> UserTopics { get; set; } = new List<Topic>();
        public List<Comment> UserComments { get; set; } = new List<Comment>();
        public bool IsDeleted { get; set; }
        public UserSettings UserSettings { get; set; } = new UserSettings();

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime JoinedDate { get; set; }
    }
}
