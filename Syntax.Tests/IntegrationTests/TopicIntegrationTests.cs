using Microsoft.AspNetCore.Authentication;
using Syntax.API.Requests;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Syntax.Tests.IntegrationTests
{
    internal class TopicIntegrationTests : IntegrationTestsBase
    {
        private readonly TestApplicationFactory _factory;
        private readonly HttpClient _client;
        private readonly string _jwt;

        public TopicIntegrationTests()
        {
            _factory = new TestApplicationFactory();
            _client = _factory.CreateClient();
        }

        [Test]
        public async Task CreateTopicAndAddCommentToIt()
        {
            var requestContent = new TopicRequest
            {
                Body = "TestBody",
                Title = "TestTitle"
            };

            await _client.PostAsync("/api/authentication/login", new SigninRequest
            {
                Username = "TimoTest",
                Password = "Testi123"
            }.ToJsonStringContent());

            var topicResponse = await _client.PostAsync("/api/topic", requestContent.ToJsonStringContent());
            var contentString = await topicResponse.Content.ReadAsStringAsync();

            if (topicResponse.IsSuccessStatusCode == false)
            {
                Assert.Fail(contentString);
            }

            var topic = DeserializeWithOptions<Topic>(contentString);

            var commentResponse = await _client.PostAsync("/api/comment", new CommentRequest
            {
                Content = "TestComment",
                TopicId = topic.Id
            }.ToJsonStringContent());

            if (commentResponse.IsSuccessStatusCode == false)
            {
                var msg = await commentResponse.Content.ReadAsStringAsync();
                Assert.Fail(msg);
            }
        }
    }
}
