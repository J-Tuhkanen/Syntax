using Syntax.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Syntax.Core.Services.Base
{
    public interface IPostService
    {
        Task<Post> CreatePostAsync(Post post);
        Task<bool> DeletePost(string id);
        Task<Post> GetPostById(string id);
        Task<IEnumerable<Post>> GetPostsByUserAsync(string userId, IEnumerable<string> excludedPosts, int amount);
        Task<IEnumerable<Post>> GetPostsAsync(IEnumerable<string> excludedPosts, int amount);
    }
}