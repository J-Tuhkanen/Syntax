using Syntax.Core.Models;

namespace Syntax.Core.Repositories.Base
{
    public interface IPostRepository
    {
        Task<Post> CreatePostAsync(Post post);
        Task<Post> DeletePostAsync(Guid id);
        Task<Post> GetPostById(Guid id);
        Task<IEnumerable<Post>> GetPostsAsync(IEnumerable<Guid> excludedPosts, int amount);
        Task<IEnumerable<Post>> GetPostsByUserAsync(string userId, IEnumerable<Guid> excludedPosts, int amount);
        Task SaveChangesAsync();
    }
}