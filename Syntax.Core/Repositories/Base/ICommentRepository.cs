using Syntax.Core.Models;

namespace Syntax.Core.Repositories.Base
{
    public interface ICommentRepository
    {
        Task<Comment> CreateCommentAsync(Comment comment);
        Task<bool> DeleteComment(string commentId);
        Task<IEnumerable<Comment>> GetCommentsAsync(string postId, IEnumerable<string> excludedComments, int amount);
        Task<IEnumerable<Comment>> GetCommentsByUserAsync(string userId, IEnumerable<string> excludedComments, int amount);
        Task SaveChangesAsync();
    }
}