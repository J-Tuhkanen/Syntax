namespace Syntax.Tests
{
    public class PostTests : TestBase
    {
        private UserAccount _user;
        private ITopicService _postService;

        [OneTimeSetUp]
        public async Task Setup()
        {
            Debug.WriteLine("Setting up tests...");
            var dbContext = GetService<ApplicationDbContext>();
            IUserService userService = GetService<IUserService>();
            _postService = GetService<ITopicService>();
            Debug.WriteLine("Ensuring database does not exist.");
            await dbContext.Database.EnsureDeletedAsync();
            await dbContext.Database.MigrateAsync();

            _user = CreateUserInstance();

            var result = await userService.CreateUser(_user, "TimoTest", "Testi123", "timo.testi@gmail.com");
        }

        [Test]
        public async Task EnsurePostHasId()
        {
            var newPost = await _postService.CreatePostAsync("Jotain", "Työmaa", _user.Id);
            Assert.IsNotNull(newPost.Id);
        }

        [Test]
        public async Task IsPostCreationSuccessful()
        {
            var newPost = await _postService.CreatePostAsync("Jotain", "Työmaa", _user.Id);
            var post = await _postService.GetTopicAsync(newPost.Id);

            Assert.IsNotNull(post);
        }

        [Test]
        public async Task CheckPostContainsDeletionFlag()
        {
            var newPost = await _postService.CreatePostAsync("Janne", "Työmies", _user.Id);

            var post = await _postService.DeleteTopicAsync(newPost.Id);

            Assert.True(post.IsDeleted);
        }

        [Test]
        public async Task CheckCreatedPostContainsTimestamp()
        {
            var newPost = await _postService.CreatePostAsync("Janne", "Työmies", _user.Id);

            var postTimeStamp = newPost.Timestamp;
            var currentTimestamp = DateTime.UtcNow;

            var isMatchingYear = currentTimestamp.Year.Equals(postTimeStamp.Year);
            var isMatchingMonth = currentTimestamp.Month.Equals(postTimeStamp.Month);
            var isMatchingDay = currentTimestamp.Day.Equals(postTimeStamp.Day);

            Assert.True(isMatchingDay && isMatchingMonth && isMatchingYear);
        }
    }
}