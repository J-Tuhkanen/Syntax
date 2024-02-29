using Syntax.Core.Models;

namespace Syntax.Core.Repositories.Base
{
    public interface ITopicRepository
    {
        Task<Topic> CreatePostAsync(Topic post);
        Task<Topic> DeletePostAsync(Guid id);
        Task<Topic> GetPostById(Guid id);
        Task<IEnumerable<Topic>> GetPostsAsync(IEnumerable<Guid> excludedPosts, int amount);
        Task<IEnumerable<Topic>> GetPostsByUserAsync(string userId, IEnumerable<Guid> excludedPosts, int amount);
        Task SaveChangesAsync();
    }
}