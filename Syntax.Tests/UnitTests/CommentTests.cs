﻿namespace Syntax.Tests.UnitTests
{
    public class CommentTests : TestBase
    {
        private UserAccount _user;
        private Topic _targetTopic;
        private ITopicService _topicService;
        private ICommentService _commentService;

        [OneTimeSetUp]
        public async Task Setup()
        {
            Debug.WriteLine("Setting up tests...");
            var dbContext = GetService<ApplicationDbContext>();
            IUserService userService = GetService<IUserService>();
            _topicService = GetService<ITopicService>();
            _commentService = GetService<ICommentService>();
            Debug.WriteLine("Ensuring database does not exist.");
            await dbContext.Database.EnsureDeletedAsync();
            await dbContext.Database.MigrateAsync();

            _user = CreateUserInstance();
            await userService.CreateUser(_user, "TimoTest", "Testi123", "timo.testi@gmail.com");
        }

        [Test]
        public async Task EnsureCommentHasId()
        {
            _targetTopic = await _topicService.CreateTopicAsync("Janne", "Työmies", _user);
            var newComment = await _commentService.CreateCommentAsync(_targetTopic.Id, "Timo Testi on hieno mies", _user);

            Assert.IsNotNull(newComment.Id);
        }

        [Test]
        public async Task IsCommentCreationSuccessful()
        {
            _targetTopic = await _topicService.CreateTopicAsync("Janne", "Työmies", _user);
            Comment newComment = await _commentService.CreateCommentAsync(_targetTopic.Id, "Timo Testi on hieno mies", _user);
            var comment = await _commentService.GetCommentAsync(newComment.Id);
            Assert.IsNotNull(comment.Id);
        }

        [Test]
        public async Task CheckCommentContainsDeletionFlag()
        {
            _targetTopic = await _topicService.CreateTopicAsync("Janne", "Työmies", _user);
            var newComment = await _commentService.CreateCommentAsync(_targetTopic.Id, "Timo Testi poistuu kentältä", _user);
            var commentToBeDeleted = await _commentService.DeleteCommentAsync(newComment.Id);

            Assert.True(commentToBeDeleted.IsDeleted);
        }

        [Test]
        public async Task CheckCreatedCommentContainsTimestamp()
        {
            _targetTopic = await _topicService.CreateTopicAsync("Janne", "Työmies", _user);
            var newComment = await _commentService.CreateCommentAsync(_targetTopic.Id, "Hei maailma!", _targetTopic.User);

            var commentTimestamp = newComment.Timestamp;
            var currentTimestamp = DateTime.UtcNow;

            var isMatchingYear = currentTimestamp.Year.Equals(commentTimestamp.Year);
            var isMatchingMonth = currentTimestamp.Month.Equals(commentTimestamp.Month);
            var isMatchingDay = currentTimestamp.Day.Equals(commentTimestamp.Day);

            Assert.True(isMatchingDay && isMatchingMonth && isMatchingYear);
        }
    }
}
