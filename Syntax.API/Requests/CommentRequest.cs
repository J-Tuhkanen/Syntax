
namespace Syntax.API.Requests
{
    public class CommentRequest
    {
        public Guid TopicId { get; set; }
        public string Content { get; set; } = null!;
    }
}
