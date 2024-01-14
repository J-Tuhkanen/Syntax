using Syntax.Core.Data;
using Syntax.Core.Models;
using Syntax.Core.Repositories;
using Syntax.Core.Repositories.Base;
using Syntax.Core.Services.Base;

namespace Syntax.Core.Services
{
    public class CommentService : ICommentService
    {
        private readonly UnitOfWork _unitOfWork;

        public CommentService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Comment> CreateCommentAsync(string postId, string content, string userId)
        {
            var comment = new Comment(postId, content, userId);

            await _unitOfWork.Comment.CreateCommentAsync(comment);
            await _unitOfWork.Comment.SaveChangesAsync();            

            return comment;
        }

        public async Task<Comment> DeleteCommentAsync(string id)
        {
            return await _unitOfWork.Comment.DeleteCommentAsync(id);
        }

        public async Task<Comment> GetCommentAsync(string id) 
            => await _unitOfWork.Comment.GetCommentAsync(id);

        public async Task<IEnumerable<Comment>> GetCommentsAsync(string postId, IEnumerable<string> ExcludedComments, int amount)
            => await _unitOfWork.Comment.GetCommentsAsync(postId, ExcludedComments, amount);

        public async Task<IEnumerable<Comment>> GetCommentsByUserAsync(string userId, IEnumerable<string> ExcludedComments, int amount)
            => await _unitOfWork.Comment.GetCommentsByUserAsync(userId, ExcludedComments, amount);
    }
}
