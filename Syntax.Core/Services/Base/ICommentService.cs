using Syntax.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Syntax.Core.Services.Interfaces
{
    public interface ICommentService
    {
        Task<Comment> CreateCommentAsync(Comment comment);
        Task<bool> DeleteComment(string id);
        Task<Comment> GetCommentById(string id);
        Task<IEnumerable<Comment>> GetCommentsAsync(string postId);
        Task<IEnumerable<Comment>> GetCommentsByUserAsync(string userId, int amount = 0);
    }
}