using System.ComponentModel.DataAnnotations;

namespace Syntax.API.Requests
{
    public class PostTopicRequest
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Body { get; set; }
    }
}
