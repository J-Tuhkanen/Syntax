using Syntax.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Syntax.Core.Services.Base
{
    public interface ITopicService
    {
        Task<Topic> CreateTopicAsync(string title, string body, UserAccount user);
        Task<Topic> DeleteTopicAsync(Guid id);
        Task<Topic> GetTopicAsync(Guid id);
        Task<IEnumerable<Topic>> GetTopicByUserAsync(string userId, IEnumerable<Guid> excludedTopics, int amount);
        Task<IEnumerable<Topic>> GetTopicsAsync(IEnumerable<Guid> excludedTopics, int amount);
    }
}