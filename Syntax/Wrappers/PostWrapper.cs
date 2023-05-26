using Syntax.Models;

namespace Syntax.Wrappers
{
    public class PostWrapper : Post
    {
        private int _maxBodyLengthAsShortened = 80;

        public string DisplayBody { get; }

        public PostWrapper(Post post)
        {
            Id = post.Id;
            UserId = post.UserId;
            Title = post.Title;
            Body = post.Body;
            IsDeleted = post.IsDeleted;
            Timestamp = post.Timestamp;

            DisplayBody = Body.Length > _maxBodyLengthAsShortened + 3 
                ? Body.Substring(0, _maxBodyLengthAsShortened) + "..." 
                : Body;
        }
    }
}
