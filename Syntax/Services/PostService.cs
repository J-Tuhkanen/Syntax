using Syntax.Data;
using Syntax.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

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

        public async Task<IEnumerable<Post>> GetPosts()
        {
            return await _appDbContext.Posts.Where(p => p.IsDeleted == false).ToListAsync();
        }

        public Post GetPostById(string id)
        {
            return _appDbContext.Posts.FirstOrDefault(p => p.IsDeleted == false && p.Id == id);
        }

        public async Task<bool> DeletePost(string id)
        {
            try
            {
                var post = _appDbContext.Posts.FirstOrDefault(p => p.Id == id);

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
