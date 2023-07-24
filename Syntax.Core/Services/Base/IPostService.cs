using Syntax.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Syntax.Core.Services.Interfaces
{
    public interface IPostService
    {
        Task<Post> CreatePostAsync(Post post);
        Task<bool> DeletePost(string id);
        Task<Post> GetPostById(string id);
        Task<IEnumerable<Post>> GetPostsByUserAsync(string userId, int amount = 0);
        Task<IEnumerable<Post>> GetPostsAsync(int skipAmount = 0, int amount = 5);
    }
}