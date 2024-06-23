using Syntax.Core.Models;

namespace Syntax.Core.Repositories.Base
{
    public interface ICommentRepository
    {
        Task<Comment?> CreateCommentAsync(Comment comment);
        Task<Comment> DeleteCommentAsync(Guid commentId);
        Task<Comment?> GetCommentAsync(Guid id);
        Task<IEnumerable<Comment>> GetCommentsAsync(Guid topicId, IEnumerable<Guid> excludedComments, int amount);
        Task<IEnumerable<Comment>> GetCommentsByUserAsync(string userId, IEnumerable<Guid> excludedComments, int amount);
        Task SaveChangesAsync();
    }
}