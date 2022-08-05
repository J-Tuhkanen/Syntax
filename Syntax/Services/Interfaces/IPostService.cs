using Syntax.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Syntax.Services
{
    public interface IPostService
    {
        Task<Post> CreatePostAsync(Post post);
        Task<bool> DeletePost(string id);
        Task<Post> GetPostById(string id);
        Task<IEnumerable<Post>> GetPostsByUserAsync(string userId, int amount = 0);
        Task<IEnumerable<Post>> GetPosts(int amount = 0);
    }
}