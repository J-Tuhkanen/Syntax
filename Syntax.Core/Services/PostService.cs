using Microsoft.EntityFrameworkCore;
using Syntax.Core.Data;
using Syntax.Core.Models;
using Syntax.Core.Repositories;
using Syntax.Core.Repositories.Base;
using Syntax.Core.Services.Base;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Syntax.Core.Services
{
    public class PostService : IPostService
    {
        private readonly UnitOfWork _unitOfWork;

        public PostService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Post> CreatePostAsync(string title, string body, string userId)
        {
            Post createdPost = await _unitOfWork.Post.CreatePostAsync(new Post(title, body, userId));

            await _unitOfWork.SaveChangesAsync();

            return createdPost;
        }

        public async Task<IEnumerable<Post>> GetPostsByUserAsync(string userId, IEnumerable<string> excludedPosts, int amount)
        {
            return await _unitOfWork.Post.GetPostsByUserAsync(userId, excludedPosts, amount);
        }

        public async Task<IEnumerable<Post>> GetPostsAsync(IEnumerable<string> excludedPosts, int amount)
        {
            return await _unitOfWork.Post.GetPostsAsync(excludedPosts, amount);
        }

        public async Task<Post> GetPostByIdAsync(string id) => await _unitOfWork.Post.GetPostById(id);

        public async Task<Post> DeletePostAsync(string id)
        {
            return await _unitOfWork.Post.DeletePostAsync(id);
        }
    }
}
