using Syntax.Core.Data;
using Syntax.Core.Models;
using Syntax.Core.Repositories;
using Syntax.Core.Repositories.Base;
using Syntax.Core.Services.Base;

namespace Syntax.Core.Services
{

    public class UserActivityService : IUserActivityService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ICommentRepository _commentRepository;
        private readonly ITopicRepository _topicRepository;

        public UserActivityService(
            ApplicationDbContext applicationDbContext,
            ICommentRepository commentRepository, 
            ITopicRepository topicRepository)
        {
            _applicationDbContext = applicationDbContext;
            _commentRepository = commentRepository;
            _topicRepository = topicRepository;
        }

        public async Task<Comment?> CreateCommentAsync(Guid topicId, string content, UserAccount user)
        {
            Topic? topic = await _topicRepository.GetTopicByIdAsync(topicId);

            if(topic == null)
            {
                return null;
            }

            var comment = new Comment
            {
                Topic = topic,
                Content = content,
                User = user
            };

            await _commentRepository.CreateCommentAsync(comment);
            await _applicationDbContext.SaveChangesAsync();

            return comment;
        }

        public async Task<Comment?> DeleteCommentAsync(Guid id)
            => await _commentRepository.DeleteCommentAsync(id);

        public async Task<Comment?> GetCommentAsync(Guid id)
            => await _commentRepository.GetCommentAsync(id);

        public async Task<IEnumerable<Comment>> GetCommentsAsync(Guid postId, IEnumerable<Guid> ExcludedComments, int amount)
            => await _commentRepository.GetCommentsAsync(postId, ExcludedComments, amount);

        public async Task<IEnumerable<Comment>> GetCommentsByUserAsync(UserAccount user, IEnumerable<Guid> ExcludedComments, int amount)
            => await _commentRepository.GetCommentsByUserAsync(user, ExcludedComments, amount);

        public async Task<Topic> CreateTopicAsync(string title, string body, UserAccount user)
        {
            Topic createdPost = await _topicRepository.CreateTopicAsync(new Topic { Title = title, Body = body, User = user });

            await _applicationDbContext.SaveChangesAsync();

            return createdPost;
        }

        public async Task<IEnumerable<Topic>> GetTopicByUserAsync(string userId, IEnumerable<Guid> excludedPosts, int amount)
        {
            return await _topicRepository.GetTopicsByUserAsync(userId, excludedPosts, amount);
        }

        public async Task<IEnumerable<Topic>> GetTopicsAsync(IEnumerable<Guid> excludedPosts, int amount)
        {
            return await _topicRepository.GetTopicsAsync(excludedPosts, amount);
        }

        public async Task<Topic?> GetTopicAsync(Guid id) => await _topicRepository.GetTopicByIdAsync(id);

        public async Task<Topic?> DeleteTopicAsync(Guid id) => await _topicRepository.DeleteTopicAsync(id);

        public async Task<Topic?> UpdateTopicAsync(Guid topicId, string title, string body, UserAccount user)
        {
            var topic = await _topicRepository.GetTopicByIdAsync(topicId);

            if (topic == null || topic.User.Id != user.Id)
            {
                return null;
            }

            topic.Title = title;
            topic.Body = body;

            await _applicationDbContext.SaveChangesAsync();
            return topic;
        }
    }
}
