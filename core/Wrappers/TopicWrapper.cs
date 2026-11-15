using Microsoft.AspNetCore.SignalR;
using Syntax.Core.Models;
using Syntax.Core.Wrappers.Base;

namespace Syntax.Core.Wrappers
{
    /// <summary>
    /// Read-only wrapper for topic-object.
    /// </summary>
    public class TopicWrapper
    {
        private int _maxBodyLengthAsShortened = 140;
        public Guid Id { get; }
        public string Body { get; }
        public string Title { get; }
        public DateTime Timestamp { get; }
        public string UserName { get; }
        public string UserId { get; }

        public TopicWrapper(Topic topic)
        {
            Id = topic.Id;
            UserId = topic.User.Id;
            UserName = topic.User.UserName;
            Title = topic.Title;
            Body = topic.Body.Length > _maxBodyLengthAsShortened + 3
                ? topic.Body.Substring(0, _maxBodyLengthAsShortened) + "..."
                : topic.Body;
            Timestamp = topic.Timestamp;
        }
    }
}
