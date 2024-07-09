using Syntax.Core.Models;
using System.Text.Json.Serialization;

namespace Syntax.Core.Dtos
{
    public class CommentDto
    {
        public Guid TopicId { get; }
        public string Username { get; }
        public string Content { get; }


        [JsonConstructor]
        public CommentDto(Guid topicId, string username, string content)
        {
            TopicId = topicId;
            Username = username;
            Content = content;
        }

        public CommentDto(Comment comment)
        {
            Username = comment.User.UserName!;
            Content = comment.Content;
            TopicId = comment.Topic.Id;
        }
    }
}
