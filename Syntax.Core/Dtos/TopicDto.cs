using Syntax.Core.Models;
using System.Text.Json.Serialization;

namespace Syntax.Core.Dtos
{
    public class TopicDto
    {
        public Guid Id { get; }
        public string Username { get; }
        public Guid UserId { get; }
        public string Content { get; }
        public string Title { get; }
        public DateTime Timestamp { get; }
        public IEnumerable<CommentDto> Comments { get; }


        [JsonConstructor]
        public TopicDto(Guid id, string username, Guid userId, string content, string title, DateTime timestamp, IEnumerable<CommentDto> comments) 
        {
            Id = id;
            Username = username;
            UserId = userId;
            Content = content;
            Title = title;
            Timestamp = timestamp;
            Comments = comments;
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
