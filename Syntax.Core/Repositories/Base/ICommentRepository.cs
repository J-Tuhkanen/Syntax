using Syntax.Core.Models;

namespace Syntax.Core.Repositories.Base
{
    public interface ICommentRepository
    {
        Task<Comment> CreateCommentAsync(Comment comment);
        Task<Comment> DeleteCommentAsync(Guid commentId);
        Task<Comment> GetCommentAsync(Guid id);
        Task<IEnumerable<Comment>> GetCommentsAsync(Guid topicId);
        Task<IEnumerable<Comment>> GetCommentsByUserAsync(string userId);
        Task SaveChangesAsync();
    }
}