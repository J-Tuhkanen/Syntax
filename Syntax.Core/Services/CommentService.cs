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

        public async Task<Comment> CreateCommentAsync(Guid topicId, string content, UserAccount user)
        {
            Topic topic = await _unitOfWork.Topics.GetTopicById(topicId);

            var comment = new Comment
            {
                Topic = topic,
                Content = content,
                User = user
            };

            await _unitOfWork.Comments.CreateCommentAsync(comment);
            await _unitOfWork.Comments.SaveChangesAsync();            

            return comment;
        }

        public async Task<Comment> DeleteCommentAsync(Guid id)
            => await _unitOfWork.Comments.DeleteCommentAsync(id);        

        public async Task<Comment?> GetCommentAsync(Guid id) 
            => await _unitOfWork.Comments.GetCommentAsync(id);

        public async Task<IEnumerable<Comment>> GetCommentsAsync(Guid postId, IEnumerable<Guid> ExcludedComments, int amount)
            => await _unitOfWork.Comments.GetCommentsAsync(postId, ExcludedComments, amount);

        public async Task<IEnumerable<Comment>> GetCommentsByUserAsync(UserAccount user, IEnumerable<Guid> ExcludedComments, int amount)
            => await _unitOfWork.Comments.GetCommentsByUserAsync(user, ExcludedComments, amount);
    }
}
