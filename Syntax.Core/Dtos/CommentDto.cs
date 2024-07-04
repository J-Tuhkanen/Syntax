using Syntax.Core.Models;

namespace Syntax.Core.Dtos
{
    public class CommentDto
    {
        public Guid TopicId { get; }
        public string Username { get; }
        public string Content { get; }

        public CommentDto(Comment comment)
        {
            Username = comment.User.UserName;
            Content = comment.Content;
            TopicId = comment.Topic.Id;
        }
    }
}
