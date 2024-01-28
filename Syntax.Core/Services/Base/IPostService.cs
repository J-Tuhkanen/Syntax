using Syntax.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Syntax.Core.Services.Base
{
    public interface IPostService
    {
        Task<Post> CreatePostAsync(string title, string body, string userId);
        Task<Post> DeletePostAsync(Guid id);
        Task<Post> GetTopicAsync(Guid id);
        Task<IEnumerable<Post>> GetPostsByUserAsync(string userId, IEnumerable<Guid> excludedPosts, int amount);
        Task<IEnumerable<Post>> GetPostsAsync(IEnumerable<Guid> excludedPosts, int amount);
    }
}