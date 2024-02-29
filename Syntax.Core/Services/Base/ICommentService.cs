using Syntax.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Syntax.Core.Services.Base
{
    public interface ICommentService
    {
        Task<Comment> CreateCommentAsync(Guid topicId, string content, string userId);
        Task<Comment> DeleteCommentAsync(Guid id);
        Task<Comment> GetCommentAsync(Guid id);
        Task<IEnumerable<Comment>> GetCommentsAsync(Guid topicId, IEnumerable<Guid> ExcludedComments, int amount = 5);
        Task<IEnumerable<Comment>> GetCommentsByUserAsync(string userId, IEnumerable<Guid> ExcludedComments, int amount = 5);
    }
}