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
            try
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Debug.WriteLine(ex.Message);
            }
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

        // TODO: Deletion flag test

        // TODO: Timestamp test
    }
}
