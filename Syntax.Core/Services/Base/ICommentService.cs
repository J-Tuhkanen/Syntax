using Syntax.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Syntax.Core.Services.Base
{
    public interface ICommentService
    {
        Task<Comment> CreateCommentAsync(string postId, string content, string userId);
        Task<bool> DeleteCommentAsync(string id);
        Task<Comment> GetCommentAsync(string id);
        Task<IEnumerable<Comment>> GetCommentsAsync(string postId, IEnumerable<string> ExcludedComments, int amount = 5);
        Task<IEnumerable<Comment>> GetCommentsByUserAsync(string userId, IEnumerable<string> ExcludedComments, int amount = 5);
    }
}