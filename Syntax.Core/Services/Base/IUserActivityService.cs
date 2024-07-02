using Syntax.Core.Models;

namespace Syntax.Core.Services.Base
{
    public interface IUserActivityService
    {
        Task<Topic> CreateTopicAsync(string title, string body, UserAccount user);
        Task<Topic?> DeleteTopicAsync(Guid id);
        Task<Topic?> GetTopicAsync(Guid id);
        Task<IEnumerable<Topic>> GetTopicByUserAsync(string userId, IEnumerable<Guid> excludedTopics, int amount);
        Task<IEnumerable<Topic>> GetTopicsAsync(IEnumerable<Guid> excludedTopics, int amount);
        Task<Topic?> UpdateTopicAsync(Guid topicId, string title, string body, UserAccount user);
        Task<Comment?> CreateCommentAsync(Guid topicId, string content, UserAccount user);
        Task<Comment?> DeleteCommentAsync(Guid id);
        Task<Comment?> GetCommentAsync(Guid id);
        Task<IEnumerable<Comment>> GetCommentsAsync(Guid topicId, IEnumerable<Guid> ExcludedComments, int amount = 5);
        Task<IEnumerable<Comment>> GetCommentsByUserAsync(UserAccount user, IEnumerable<Guid> ExcludedComments, int amount = 5);

    }
}
