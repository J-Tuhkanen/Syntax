using Syntax.Core.Models;

namespace Syntax.Core.Repositories.Base
{
    public interface ITopicRepository
    {
        Task<Topic> CreateTopicAsync(Topic post);
        Task<Topic?> DeleteTopicAsync(Guid id);
        Task<Topic?> GetTopicById(Guid id);
        Task<IEnumerable<Topic>> GetTopicsAsync(IEnumerable<Guid> excludedPosts, int amount);
        Task<IEnumerable<Topic>> GetTopicsByUserAsync(string userId, IEnumerable<Guid> excludedPosts, int amount);
        Task SaveChangesAsync();
    }
}