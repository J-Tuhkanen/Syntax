using Syntax.Core.Models;

namespace Syntax.Core.Wrappers
{
    public class CommentWrapper : Comment
    {
        public CommentWrapper(Comment comment)
        {
            PostId = comment.PostId;
            UserId = comment.UserId;
            Content = comment.Content;
            IsDeleted = comment.IsDeleted;
            Timestamp = comment.Timestamp;
        }
    }
}
