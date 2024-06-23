
namespace Syntax.API.Requests
{
    public class CommentRequest
    {
        public Guid TopicId { get; internal set; }
        public string Content { get; internal set; } = null!;
    }
}
