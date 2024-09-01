using Syntax.Core.Models;
using System.Text.Json.Serialization;

namespace Syntax.Core.Dtos
{
    public class TopicDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public Guid UserId { get; set; }
        public string Content { get; set; }
        public string Title { get; set; }
        public DateTime Timestamp { get; set; }
        public IEnumerable<CommentDto> Comments { get; set; }


        [JsonConstructor]
        public TopicDto()
        {
        }

        public TopicDto(Topic topic) 
        { 
            Id = topic.Id;
            Username = topic.User.UserName;
            Comments = topic.Comments.Select(c => new CommentDto(c));
            UserId = new Guid(topic.User.Id);
            Content = topic.Body;
            Title = topic.Title;
            Timestamp = topic.Timestamp;
        }
    }
}
