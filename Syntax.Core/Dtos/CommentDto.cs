using Syntax.Core.Models;
using System.Text.Json.Serialization;

namespace Syntax.Core.Dtos
{
    public class CommentDto
    {
        public Guid? TopicId { get; set; }
        public string Username { get; set; }
        public string Content { get; set; }


        [JsonConstructor]
        public CommentDto()
        {
        }

        public CommentDto(Comment comment)
        {
            Username = comment.User.UserName!;
            Content = comment.Content;
            TopicId = comment.Topic?.Id;
        }
    }
}
