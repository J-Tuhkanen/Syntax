using Microsoft.AspNetCore.SignalR;
using Syntax.Core.Models;
using Syntax.Core.Wrappers.Base;

namespace Syntax.Core.Wrappers
{
    /// <summary>
    /// Read-only wrapper for post-object.
    /// </summary>
    public class PostWrapper
    {
        private int _maxBodyLengthAsShortened = 140;

        public string Id { get; }
        public string Body { get; }
        public string Title { get; }
        public DateTime Timestamp { get; }
        public string UserName { get; }
        public string UserId { get; }

        public PostWrapper(Post post)
        {
            Id = post.Id;
            UserId = post.User.Id;
            UserName = post.User.UserName;
            Title = post.Title;
            Body = post.Body.Length > _maxBodyLengthAsShortened + 3
                ? post.Body.Substring(0, _maxBodyLengthAsShortened) + "..."
                : post.Body;
            Timestamp = post.Timestamp;
        }
    }
}
