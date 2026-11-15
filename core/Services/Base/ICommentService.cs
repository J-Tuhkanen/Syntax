using Syntax.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Syntax.Core.Services.Base
{
    public interface ICommentService
    {
        Task<Comment?> CreateCommentAsync(Guid topicId, string content, UserAccount user);
        Task<Comment?> DeleteCommentAsync(Guid id, UserAccount user);
        Task<Comment?> GetCommentAsync(Guid id);
        Task<IEnumerable<Comment>> GetCommentsAsync(Guid topicId, IEnumerable<Guid> ExcludedComments, int amount = 5);
        Task<IEnumerable<Comment>> GetCommentsByUserAsync(UserAccount user, IEnumerable<Guid> ExcludedComments, int amount = 5);
    }
}