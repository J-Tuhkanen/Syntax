using Syntax.Core.Models;

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

        public TopicDto(Topic topic) 
        { 
            Id = topic.Id;
            Username = topic.User.UserName;
            UserId = new Guid(topic.User.Id);
            Content = topic.Body;
            Title = topic.Title;
            Timestamp = topic.Timestamp;
        }
    }
}
