using Syntax.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Syntax.Core.Services.Base
{
    public interface IPostService
    {
        Task<Post> CreatePostAsync(string title, string body, string userId);
        Task<Post> DeletePostAsync(string id);
        Task<Post> GetPostByIdAsync(string id);
        Task<IEnumerable<Post>> GetPostsByUserAsync(string userId, IEnumerable<string> excludedPosts, int amount);
        Task<IEnumerable<Post>> GetPostsAsync(IEnumerable<string> excludedPosts, int amount);
    }
}