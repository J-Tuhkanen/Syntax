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
        Task<IEnumerable<Post>> GetPosts();
    }
}