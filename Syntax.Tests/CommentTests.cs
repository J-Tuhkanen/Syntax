namespace Syntax.Tests
{
    public class CommentTests : TestBase
    {
        private UserAccount _user;
        private Post _targetPost;
        private IPostService _postService;
        private ICommentService _commentService;

        [OneTimeSetUp]
        public async Task Setup()
        {
            Debug.WriteLine("Setting up tests...");
            var dbContext = GetService<ApplicationDbContext>();
            IUserService userService = GetService<IUserService>();
            _postService = GetService<IPostService>();
            _commentService = GetService<ICommentService>();
            Debug.WriteLine("Ensuring database does not exist.");
            await dbContext.Database.EnsureDeletedAsync();
            await dbContext.Database.MigrateAsync();

            _user = CreateUserInstance();
            _targetPost = await _postService.CreatePostAsync("Janne", "Työmies", _user.Id);

            var result = await userService.CreateUser(_user, "TimoTest", "Testi123", "timo.testi@gmail.com");
        }

        [Test]
        public async Task EnsureCommentHasId()
        {
            var newComment = await _commentService.CreateCommentAsync(_targetPost.Id, "Timo Testi on hieno mies", _user.Id);

            Assert.IsNotNull(newComment.Id);
        }

        [Test]
        public async Task IsCommentCreationSuccessful()
        {
            var newComment = await _commentService.CreateCommentAsync(_targetPost.Id, "Timo Testi on hieno mies", _user.Id);
            var comment = await _commentService.GetCommentAsync(newComment.Id);
            Assert.IsNotNull(comment.Id);
        }

        [Test]
        public async Task CheckCommentContainsDeletionFlag()
        {
            var newComment = await _commentService.CreateCommentAsync(_targetPost.Id, "Timo Testi poistuu kentältä", _user.Id);
            var commentToBeDeleted = await _commentService.DeleteCommentAsync(newComment.Id);

            Assert.True(commentToBeDeleted.IsDeleted);
        }

        [Test]
        public async Task CheckCreatedCommentContainsTimestamp()
        {
            var newComment = await _commentService.CreateCommentAsync(_targetPost.Id, "Hei maailma!", _targetPost.User.Id);

            var commentTimestamp = newComment.Timestamp;
            var currentTimestamp = DateTime.UtcNow;

            var isMatchingYear = currentTimestamp.Year.Equals(commentTimestamp.Year);
            var isMatchingMonth = currentTimestamp.Month.Equals(commentTimestamp.Month);
            var isMatchingDay = currentTimestamp.Day.Equals(commentTimestamp.Day);

            Assert.True(isMatchingDay && isMatchingMonth && isMatchingYear);
        }
    }
}
