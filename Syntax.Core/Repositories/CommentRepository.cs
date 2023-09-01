using Microsoft.EntityFrameworkCore;
using Syntax.Core.Data;
using Syntax.Core.Models;
using Syntax.Core.Repositories.Base;
using System.ComponentModel.Design;

namespace Syntax.Core.Repositories
{
    public class CommentRepository : RepositoryBase, ICommentRepository
    {
        public CommentRepository(ApplicationDbContext applicationDbContext) 
            : base(applicationDbContext) 
        { 
        }
    
        public async Task<Comment> CreateCommentAsync(Comment comment)
        {
            await applicationDbContext.Comments.AddAsync(comment);

            return comment;
        }

        public async Task<bool> DeleteComment(string commentId)
        {
            Comment comment = await applicationDbContext.Comments.FirstOrDefaultAsync(c => c.Id == commentId);

            if (comment != null)
                comment.IsDeleted = true;

            return comment?.IsDeleted ?? false;
        }

        public async Task<Comment> GetCommentAsync(string id) 
            => await applicationDbContext.Comments.FirstOrDefaultAsync(c => c.Id == id);

        public async Task<IEnumerable<Comment>> GetCommentsAsync(string postId, IEnumerable<string> ExcludedComments, int amount)
        {
            var validComments = applicationDbContext.Comments.Where(c =>
                c.PostId == postId &&
                c.IsDeleted == false &&
                ExcludedComments.Contains(c.Id) == false);

            return await validComments.Take(amount).ToListAsync();
        }

        public async Task<IEnumerable<Comment>> GetCommentsByUserAsync(string userId, IEnumerable<string> ExcludedComments, int amount)
        {
            var validComments = applicationDbContext.Comments.Where(c =>
                c.UserId == userId &&
                c.IsDeleted == false &&
                ExcludedComments.Contains(c.Id) == false);

            return await validComments.Take(amount).ToListAsync();
        }
    }
}
