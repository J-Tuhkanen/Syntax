using Microsoft.EntityFrameworkCore;
using Syntax.Core.Data;
using Syntax.Core.Models;
using Syntax.Core.Services.Base;

namespace Syntax.Core.Services
{
    public class TopicService : ITopicService
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public TopicService(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<Topic> CreateTopicAsync(string title, string body, UserAccount user)
        {
            var topic = new Topic { Title = title, Body = body, User = user };

            await _applicationDbContext.Topics.AddAsync(topic);
            await _applicationDbContext.SaveChangesAsync();

            return topic;
        }

        public async Task<IEnumerable<Topic>> GetTopicByUserAsync(string userId, IEnumerable<Guid> excludedPosts, int amount)
        {
            var validPosts = _applicationDbContext.Topics
                .Include(t => t.Comments)
                .Include(t => t.User)
                .Where(p => p.IsDeleted == false && p.User.Id == userId && excludedPosts.Contains(p.Id) == false).Take(amount);

            return await validPosts.ToListAsync();
        }

        public async Task<IEnumerable<Topic>> GetTopicsAsync(IEnumerable<Guid> excludedPosts, int amount)
        {
            var validPosts = await _applicationDbContext.Topics
                .Where(p => p.IsDeleted == false && excludedPosts.Contains(p.Id) == false)
                .Take(amount)
                .Include(t => t.User)
                .Include(t => t.Comments)
                .ToListAsync();

            return validPosts;
        }

        public async Task<Topic?> GetTopicAsync(Guid id)
        {
            return await _applicationDbContext.Topics
                .AsNoTracking()
                .AsSplitQuery()
                .Include(t => t.User)
                .Include(t => t.Comments)
                .FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);
        }

        public async Task<Topic?> DeleteTopicAsync(Guid id, UserAccount user)
        {
            // Get post from database
            var topic = _applicationDbContext.Topics
                .AsSplitQuery()
                .FirstOrDefault(p => p.IsDeleted == false && p.Id == id && p.User.Id == user.Id);

            if (topic == null)
                return null;

            // If post was found, mark it as deleted.
            topic.IsDeleted = true;
            await _applicationDbContext.SaveChangesAsync();

            return topic;
        }        
    }
}
