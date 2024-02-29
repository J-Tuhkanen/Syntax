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

        public async Task<Comment> CreateCommentAsync(Guid postId, string content, string userId)
        {
            Topic post = await _unitOfWork.Topics.GetPostById(postId);

            var comment = new Comment
            {
                Topic = post,
                Content = content,
                UserId = userId
            };

            await _unitOfWork.Comment.CreateCommentAsync(comment);
            await _unitOfWork.Comment.SaveChangesAsync();            

            return comment;
        }

        public async Task<Comment> DeleteCommentAsync(Guid id)
        {
            return await _unitOfWork.Comment.DeleteCommentAsync(id);
        }

        public async Task<Comment> GetCommentAsync(Guid id) 
            => await _unitOfWork.Comment.GetCommentAsync(id);

        public async Task<IEnumerable<Comment>> GetCommentsAsync(Guid postId, IEnumerable<Guid> ExcludedComments, int amount)
            => await _unitOfWork.Comment.GetCommentsAsync(postId, ExcludedComments, amount);

        public async Task<IEnumerable<Comment>> GetCommentsByUserAsync(string userId, IEnumerable<Guid> ExcludedComments, int amount)
            => await _unitOfWork.Comment.GetCommentsByUserAsync(userId, ExcludedComments, amount);
    }
}
