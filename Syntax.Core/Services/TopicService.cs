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
    public class TopicService : ITopicService
    {
        private readonly UnitOfWork _unitOfWork;

        public TopicService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Topic> CreatePostAsync(string title, string body, string userId)
        {
            Topic createdPost = await _unitOfWork.Post.CreatePostAsync(new Topic { Title = title, Body = body, UserId = userId });

            await _unitOfWork.SaveChangesAsync();

            return createdPost;
        }

        public async Task<IEnumerable<Topic>> GetPostsByUserAsync(string userId, IEnumerable<Guid> excludedPosts, int amount)
        {
            return await _unitOfWork.Post.GetPostsByUserAsync(userId, excludedPosts, amount);
        }

        public async Task<IEnumerable<Topic>> GetPostsAsync(IEnumerable<Guid> excludedPosts, int amount)
        {
            return await _unitOfWork.Post.GetPostsAsync(excludedPosts, amount);
        }

        public async Task<Topic> GetTopicAsync(Guid id) => await _unitOfWork.Post.GetPostById(id);

        public async Task<Topic> DeletePostAsync(Guid id)
        {
            return await _unitOfWork.Post.DeletePostAsync(id);
        }
    }
}
