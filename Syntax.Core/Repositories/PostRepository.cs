using Microsoft.EntityFrameworkCore;
using Syntax.Core.Data;
using Syntax.Core.Models;
using Syntax.Core.Repositories.Base;

namespace Syntax.Core.Repositories
{
    internal class PostRepository : RepositoryBase, IPostRepository
    {
        internal PostRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
        }

        public async Task<Post> CreatePostAsync(Post post)
        {
            await applicationDbContext.Posts.AddAsync(post);

            return post;
        }

        public async Task<Post> DeletePostAsync(string id)
        {
            // Get post from database
            var post = applicationDbContext.Posts.FirstOrDefault(p => p.Id == id);

            // If post was found, mark it as deleted.
            if (post == null)
            {
                throw new Exception($"Post with id {id} was not found and could not be deleted.");
            }

            post.IsDeleted = true;
            await applicationDbContext.SaveChangesAsync();

            return post;
        }

        public async Task<IEnumerable<Post>> GetPostsByUserAsync(string userId, IEnumerable<string> excludedPosts, int amount)
        {
            var validPosts = applicationDbContext.Posts.Where(p =>
                p.IsDeleted == false &&
                p.UserId == userId &&
                excludedPosts.Contains(p.Id) == false).Take(amount);

            return await validPosts.ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetPostsAsync(IEnumerable<string> excludedPosts, int amount)
        {
            var validPosts = await applicationDbContext.Posts.Where(p =>
                p.IsDeleted == false &&
                excludedPosts.Contains(p.Id) == false).Take(amount).ToListAsync();
            
            return validPosts;
        }

        public async Task<Post> GetPostById(string id) => await applicationDbContext.Posts.FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);

    }
}
