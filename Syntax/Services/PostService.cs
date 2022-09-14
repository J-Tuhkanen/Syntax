using Microsoft.EntityFrameworkCore;
using Syntax.Data;
using Syntax.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Syntax.Services
{
    public class PostService : IPostService
    {
        private ApplicationDbContext _appDbContext;

        public PostService(ApplicationDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            
        }

        public async Task<Post> CreatePostAsync(Post post)
        {
            try
            {
                await _appDbContext.Posts.AddAsync(post);
                await _appDbContext.SaveChangesAsync();

                return post;
            }
            catch
            {
                // Add logging
                return null;
            }
        }

        public async Task<IEnumerable<Post>> GetPostsByUserAsync(string userId, int amount)
        {
            var postsByUser = _appDbContext.Posts.Where(p => p.UserId == userId);

            return amount > 0 
                ? await postsByUser.Take(amount).ToListAsync() 
                : await postsByUser.ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetPosts(int skipAmount, int amount)
        {
            var notDeletedPosts = _appDbContext.Posts.Where(p => p.IsDeleted == false);

            return amount > 0
                ? await notDeletedPosts.ToListAsync()
                : await notDeletedPosts.Skip(skipAmount).Take(amount).ToListAsync();
        }

        public async Task<Post> GetPostById(string id)
        {
            return await _appDbContext.Posts.FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);
        }        

        /// <summary>
        /// Mark post as deleted by post id
        /// </summary>
        public async Task<bool> DeletePost(string id)
        {
            try
            {
                // Get post from database
                var post = _appDbContext.Posts.FirstOrDefault(p => p.Id == id);

                // If post was found, mark it as deleted.
                if (post != null)
                {
                    post.IsDeleted = true;
                    await _appDbContext.SaveChangesAsync();

                    return true;
                }

                return false;
            }
            catch
            {
                // TODO: Add logging.
                return false;
            }
        }
    }
}
