using Microsoft.EntityFrameworkCore;
using Syntax.Core.Data;
using Syntax.Core.Models;
using Syntax.Core.Services.Base;

namespace Syntax.Core.Services
{
    public class CommentService : ICommentService
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public CommentService(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext; ;
        }

        public async Task<Comment?> CreateCommentAsync(Guid topicId, string content, UserAccount user)
        {
            var topic = await _applicationDbContext.Topics.FirstOrDefaultAsync(t => t.IsDeleted == false && t.Id == topicId);

            if (topic == null)
                return null;            

            var comment = new Comment
            {
                Content = content,
                User = user,
                Topic = topic
            };

            await _applicationDbContext.Comments.AddAsync(comment);
            await _applicationDbContext.SaveChangesAsync();

            return comment;
        }

        public async Task<Comment?> DeleteCommentAsync(Guid id, UserAccount user)
        {
            var comment = _applicationDbContext.Comments.FirstOrDefault(comment => comment.Id == id && comment.IsDeleted == false && comment.User.Id == user.Id);

            if (comment == null)
                return null;

            comment.IsDeleted = true;
            await _applicationDbContext.SaveChangesAsync();

            return comment;
        }

        public async Task<Comment?> GetCommentAsync(Guid id)
        {
            return await _applicationDbContext.Comments
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Comment>> GetCommentsAsync(Guid postId, IEnumerable<Guid> ExcludedComments, int amount)
        {
            IQueryable<Comment> query = _applicationDbContext.Comments
                .Include(c => c.User)
                .Include(c => c.Topic)
                .AsNoTracking()
                .AsSplitQuery()
                .Where(c => c.Topic.Id == postId && c.IsDeleted == false && ExcludedComments.Contains(c.Id) == false);

            return await query.Take(amount).ToListAsync();
        }

        public async Task<IEnumerable<Comment>> GetCommentsByUserAsync(UserAccount user, IEnumerable<Guid> ExcludedComments, int amount)
        {
            IQueryable<Comment> query = _applicationDbContext.Comments
                .AsSplitQuery()
                .AsNoTracking()
                .Where(c => c.User == user && c.IsDeleted == false && ExcludedComments.Contains(c.Id) == false);

            return await query.Take(amount).ToListAsync();
        }
    }
}
