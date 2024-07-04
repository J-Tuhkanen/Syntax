using System.ComponentModel.DataAnnotations;

namespace Syntax.API.Requests
{
    public class TopicRequest
    {
        [Required]
        public string Title { get; set; } = null!;
        [Required]
        public string Body { get; set; } = null!;
    }
}
