using Syntax.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Syntax.Services
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