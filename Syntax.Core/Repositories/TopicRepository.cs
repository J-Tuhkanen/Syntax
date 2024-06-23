using Microsoft.EntityFrameworkCore;
using Syntax.Core.Data;
using Syntax.Core.Models;
using Syntax.Core.Repositories.Base;

namespace Syntax.Core.Repositories
{
    internal class TopicRepository : RepositoryBase, ITopicRepository
    {
        internal TopicRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
        }

        public async Task<Topic> CreateTopicAsync(Topic post)
        {
            await applicationDbContext.Topics.AddAsync(post);

            return post;
        }

        public async Task<Topic> DeleteTopicAsync(Guid id)
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

        public async Task<IEnumerable<Topic>> GetTopicsByUserAsync(string userId, IEnumerable<Guid> excludedPosts, int amount)
        {
            var validPosts = applicationDbContext.Topics.Where(p =>
                p.IsDeleted == false &&
                p.User.Id == userId &&
                excludedPosts.Contains(p.Id) == false).Take(amount);

            return await validPosts.ToListAsync();
        }

        public async Task<IEnumerable<Topic>> GetTopicsAsync(IEnumerable<Guid> excludedPosts, int amount)
        {
            var validPosts = await applicationDbContext.Topics
                .Where(p => p.IsDeleted == false && excludedPosts.Contains(p.Id) == false)
                .Take(amount)
                .Include(t => t.User)
                .ToListAsync();
            
            return validPosts;
        }

        public async Task<Topic> GetTopicById(Guid id) => await applicationDbContext.Topics.FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);

    }
}
