using Microsoft.EntityFrameworkCore;
using Syntax.Core.Data;
using Syntax.Core.Models;
using Syntax.Core.Repositories.Base;
using System.ComponentModel.Design;

namespace Syntax.Core.Repositories
{
    internal class CommentRepository : RepositoryBase, ICommentRepository
    {
        internal CommentRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext) 
        { 
        }
    
        public async Task<Comment> CreateCommentAsync(Comment comment)
        {
            await applicationDbContext.Comments.AddAsync(comment);

            return comment;
        }

        public async Task<Comment> DeleteCommentAsync(Guid id)
        {
            var comment = applicationDbContext.Comments.FirstOrDefault(comment => comment.Id == id);

            if (comment == null)
            {
                throw new Exception($"Comment with id {id} was not found and could not be deleted.");
            }

            comment.IsDeleted = true;
            await applicationDbContext.SaveChangesAsync();

            return comment;
        }

        public async Task<Comment> GetCommentAsync(Guid id)
        {
            return await applicationDbContext.Comments.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Comment>> GetCommentsAsync(Guid postId)
        {
            IQueryable<Comment> query = applicationDbContext.Comments.Where(c =>
                c.Topic.Id == postId &&
                c.IsDeleted == false);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Comment>> GetCommentsByUserAsync(string userId)
        {
            IQueryable<Comment> query = applicationDbContext.Comments.Where(c =>
                c.UserId == userId &&
                c.IsDeleted == false);

            return await query.ToListAsync();
        }
    }
}
