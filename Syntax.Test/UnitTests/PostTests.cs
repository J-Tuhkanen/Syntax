namespace Syntax.Tests.UnitTests
{
    public class TopicTests : TestBase
    {
        private UserAccount _user;
        private ITopicService _topicService;

       
        public async Task Setup()
        {
            Debug.WriteLine("Setting up tests...");
            var dbContext = GetService<ApplicationDbContext>();
            IUserService userService = GetService<IUserService>();
            _topicService = GetService<ITopicService>();
            Debug.WriteLine("Ensuring database does not exist.");
            await dbContext.Database.EnsureDeletedAsync();
            await dbContext.Database.MigrateAsync();

            _user = CreateUserInstance();

            var result = await userService.CreateUser(_user, "TimoTest", "Testi123", "timo.testi@gmail.com");
        }

        [Fact]
        public async Task EnsureTopicHasId()
        {
            await Setup();

            var newTopic = await _topicService.CreateTopicAsync("Jotain", "Työmaa", _user);
            Assert.NotNull(newTopic);
        }

        [Fact]
        public async Task IsTopicCreationSuccessful()
        {
            await Setup();

            var newTopic = await _topicService.CreateTopicAsync("Jotain", "Työmaa", _user);
            var topic = await _topicService.GetTopicAsync(newTopic.Id);

            Assert.NotNull(topic);
        }

        [Fact]
        public async Task CheckTopicContainsDeletionFlag()
        {
            await Setup();

            var newTopic = await _topicService.CreateTopicAsync("Janne", "Ty�mies", _user);
            var topic = await _topicService.DeleteTopicAsync(newTopic.Id, _user);

            Assert.True(topic.IsDeleted);
        }

        [Fact]
        public async Task CheckCreatedTopicContainsTimestamp()
        {
            await Setup();

            var newTopic = await _topicService.CreateTopicAsync("Janne", "Työmies", _user);

            var topicTimeStamp = newTopic.Timestamp;
            var currentTimestamp = DateTime.UtcNow;

            var isMatchingYear = currentTimestamp.Year.Equals(topicTimeStamp.Year);
            var isMatchingMonth = currentTimestamp.Month.Equals(topicTimeStamp.Month);
            var isMatchingDay = currentTimestamp.Day.Equals(topicTimeStamp.Day);

            Assert.True(isMatchingDay && isMatchingMonth && isMatchingYear);
        }
    }
}