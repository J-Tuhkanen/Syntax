using Microsoft.EntityFrameworkCore;
using Syntax.Data;
using Syntax.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Syntax.Services
{
    public class CommentService : ICommentService
    {
        private ApplicationDbContext _appDbContext;

        public CommentService(ApplicationDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<Comment> CreateCommentAsync(Comment comment)
        {
            await _appDbContext.Comments.AddAsync(comment);
            await _appDbContext.SaveChangesAsync();

            return comment;
        }

        public async Task<bool> DeleteComment(string id)
        {
            var comment = await _appDbContext.Comments.FirstOrDefaultAsync(c => c.Id == id);

            if (comment != null)
            {
                comment.IsDeleted = true;
                await _appDbContext.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task<Comment> GetCommentById(string id)
        {
            return await _appDbContext.Comments.FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);
        }

        public async Task<IEnumerable<Comment>> GetCommentsAsync(string postId)
        {
            return await _appDbContext.Comments.Where(c => c.PostId == postId && c.IsDeleted == false).ToListAsync();
        }
    }
}
