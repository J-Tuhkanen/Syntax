using Syntax.Core.Models;

namespace Syntax.Core.Services.Base
{
    public interface ICommentService
    {
        Task<Comment> CreateCommentAsync(Guid topicId, string content, string userId);
        Task<Comment> DeleteCommentAsync(Guid id);
        Task<Comment> GetCommentAsync(Guid id);
        Task<IEnumerable<Comment>> GetCommentsAsync(Guid topicId);
        Task<IEnumerable<Comment>> GetCommentsByUserAsync(string userId);
    }
}