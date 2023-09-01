using Microsoft.EntityFrameworkCore;
using Syntax.Core.Data;
using Syntax.Core.Models;
using Syntax.Core.Repositories;
using Syntax.Core.Repositories.Base;
using Syntax.Core.Services.Base;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Syntax.Core.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private ApplicationDbContext _appDbContext;

        public PostService(ApplicationDbContext appDbContext, IPostRepository postRepository)
        {
            _postRepository = postRepository;
            _appDbContext = appDbContext;
        }

        public async Task<Post> CreatePostAsync(string title, string body, string userId)
        {
            Post createdPost = await _postRepository.CreatePostAsync(new Post(title, body, userId));

            await _postRepository.SaveChangesAsync();

            return createdPost;
        }

        public async Task<IEnumerable<Post>> GetPostsByUserAsync(string userId, IEnumerable<string> excludedPosts, int amount)
        {
            return await _postRepository.GetPostsByUserAsync(userId, excludedPosts, amount);
        }

        public async Task<IEnumerable<Post>> GetPostsAsync(IEnumerable<string> excludedPosts, int amount)
        {
            return await _postRepository.GetPostsAsync(excludedPosts, amount);
        }

        public async Task<Post> GetPostByIdAsync(string id) => await _postRepository.GetPostById(id);


        /// <summary>
        /// Mark post as deleted by post id
        /// </summary>
        public async Task<Post> DeletePostAsync(string id)
        {
            // Get post from database
            var post = _appDbContext.Posts.FirstOrDefault(p => p.Id == id);

            // If post was found, mark it as deleted.
            if (post == null)
            {
                throw new Exception($"Post with id {id} was not found and could not be deleted.");
            }

            post.IsDeleted = true;
            await _appDbContext.SaveChangesAsync();

            return post;
        }
    }
}
