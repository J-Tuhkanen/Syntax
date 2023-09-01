using Syntax.Core.Models;
using Syntax.Core.Repositories.Base;
using Syntax.Core.Services.Base;

namespace Syntax.Core.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;

        public CommentService(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        public async Task<Comment> CreateCommentAsync(string postId, string content, string userId)
        {
            var comment = new Comment(postId, content, userId);

            await _commentRepository.CreateCommentAsync(comment);
            await _commentRepository.SaveChangesAsync();            

            return comment;
        }

        public async Task<bool> DeleteCommentAsync(string id)
        {
            var isSuccessful = await _commentRepository.DeleteComment(id);
            await _commentRepository.SaveChangesAsync();

            return isSuccessful;
        }

        public async Task<Comment> GetCommentAsync(string id) 
            => await _commentRepository.GetCommentAsync(id);

        public async Task<IEnumerable<Comment>> GetCommentsAsync(string postId, IEnumerable<string> ExcludedComments, int amount)
            => await _commentRepository.GetCommentsAsync(postId, ExcludedComments, amount);

        public async Task<IEnumerable<Comment>> GetCommentsByUserAsync(string userId, IEnumerable<string> ExcludedComments, int amount)
            => await _commentRepository.GetCommentsByUserAsync(userId, ExcludedComments, amount);
    }
}
