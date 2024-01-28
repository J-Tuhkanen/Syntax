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

        public async Task<Topic> CreatePostAsync(Topic post)
        {
            await applicationDbContext.Topics.AddAsync(post);

            return post;
        }

        public async Task<Topic> DeletePostAsync(Guid id)
        {
            // Get post from database
            var post = applicationDbContext.Topics.FirstOrDefault(p => p.Id == id);

            // If post was found, mark it as deleted.
            if (post == null)
                throw new Exception($"Post with id {id} was not found and could not be deleted.");
            
            post.IsDeleted = true;
            await applicationDbContext.SaveChangesAsync();

            return post;
        }

        public async Task<IEnumerable<Topic>> GetPostsByUserAsync(string userId, IEnumerable<Guid> excludedPosts, int amount)
        {
            var validPosts = applicationDbContext.Topics.Where(p =>
                p.IsDeleted == false &&
                p.UserId == userId &&
                excludedPosts.Contains(p.Id) == false).Take(amount);

            return await validPosts.ToListAsync();
        }

        public async Task<IEnumerable<Topic>> GetPostsAsync(IEnumerable<Guid> excludedPosts, int amount)
        {
            var validPosts = await applicationDbContext.Topics.Where(p =>
                p.IsDeleted == false &&
                excludedPosts.Contains(p.Id) == false).Take(amount).ToListAsync();
            
            return validPosts;
        }

        public async Task<Topic> GetPostById(Guid id) => await applicationDbContext.Topics.FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);

    }
}
