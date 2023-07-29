using Syntax.Core.Models;

namespace Syntax.Core.Repositories.Base
{
    public interface IPostRepository
    {
        Task<Post> CreatePostAsync(Post post);
        Task<Post> GetPostById(string id);
        Task<IEnumerable<Post>> GetPostsAsync(IEnumerable<string> excludedPosts, int amount);
        Task<IEnumerable<Post>> GetPostsByUserAsync(string userId, IEnumerable<string> excludedPosts, int amount);
        Task SaveChangesAsync();
    }
}