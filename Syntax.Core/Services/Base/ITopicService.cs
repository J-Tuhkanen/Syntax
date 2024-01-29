using Syntax.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Syntax.Core.Services.Base
{
    public interface ITopicService
    {
        Task<Topic> CreateTopicAsync(string title, string body, string userId);
        Task<Topic> DeleteTopicAsync(Guid id);
        Task<Topic> GetTopicAsync(Guid id);
        Task<IEnumerable<Topic>> GetPostsByUserAsync(string userId, IEnumerable<Guid> excludedPosts, int amount);
        Task<IEnumerable<Topic>> GetPostsAsync(IEnumerable<Guid> excludedPosts, int amount);
    }
}