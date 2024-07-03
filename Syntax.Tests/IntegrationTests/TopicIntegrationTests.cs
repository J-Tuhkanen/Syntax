using Microsoft.EntityFrameworkCore.Query;
using Syntax.API.Requests;
using System.Net;

namespace Syntax.Tests.IntegrationTests
{
    internal class TopicIntegrationTests : IntegrationTestsBase
    {
        [Test]
        public async Task CreateTopicAndAddCommentToIt()
        {
            var client = await CreateClientAndAuthenticate(TimoTestUsername);

            var requestContent = new TopicRequest
            {
                Body = "TestBody",
                Title = "TestTitle"
            };

            var topicResponse = await client.PostAsync("/api/topic", requestContent.ToJsonStringContent());
            var contentString = await topicResponse.Content.ReadAsStringAsync();

            if (topicResponse.IsSuccessStatusCode == false)
            {
                Assert.Fail(contentString);
            }

            var topic = DeserializeWithOptions<Topic>(contentString);

            var commentResponse = await client.PostAsync("/api/comment", new CommentRequest
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

        [Test]
        public async Task CreateTopicAndGetTopicWithId()
        {
            var client = await CreateClientAndAuthenticate(TimoTestUsername);

            var requestContent = new TopicRequest
            {
                Body = "TestBody",
                Title = "TestTitle"
            };

            var postTopicResponse = await client.PostAsync("/api/topic", requestContent.ToJsonStringContent());
            var postTopic = DeserializeWithOptions<Topic>(await postTopicResponse.Content.ReadAsStringAsync());

            var commentResponse = await client.PostAsync("/api/comment", new CommentRequest
            {
                Content = "TestComment",
                TopicId = postTopic.Id
            }.ToJsonStringContent());

            var getTopicResponse = await client.GetAsync($"/api/topic/{postTopic.Id}");
            var getTopic = DeserializeWithOptions<Topic>(await getTopicResponse.Content.ReadAsStringAsync());

            Assert.That(getTopic.Title, Is.EqualTo(postTopic.Title));
            Assert.That(getTopic.Body, Is.EqualTo(postTopic.Body));
            Assert.True(getTopic.Comments.Count == 1);
            Assert.That(getTopic.User.UserName, Is.EqualTo(TestApplicationFactory.TimoTestUser.UserName));
            Assert.That(getTopic.User.Email, Is.EqualTo(TestApplicationFactory.TimoTestUser.Email));
        }

        [Test]
        public async Task CreateTopicAndDelete()
        {
            var client = await CreateClientAndAuthenticate(TimoTestUsername);

            var requestContent = new TopicRequest
            {
                Body = "TestBody",
                Title = "TestTitle"
            };

            var postTopicResponse = await client.PostAsync("/api/topic", requestContent.ToJsonStringContent());
            var postTopic = DeserializeWithOptions<Topic>(await postTopicResponse.Content.ReadAsStringAsync());

            await client.DeleteAsync($"/api/topic/{postTopic.Id}");

            var response = await client.GetAsync($"/api/topic/{postTopic.Id}");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task CreateTopicAndOtherAccountTriesToDelete()
        {
            var jutska = QueryableMethods.Join;

            var timoClient = await CreateClientAndAuthenticate(TimoTestUsername);
            var toniClient = await CreateClientAndAuthenticate(ToniTestUsername);

            var requestContent = new TopicRequest
            {
                Body = "TestBody",
                Title = "TestTitle"
            };

            var postTopicResponse = await timoClient.PostAsync("/api/topic", requestContent.ToJsonStringContent());
            var postTopic = DeserializeWithOptions<Topic>(await postTopicResponse.Content.ReadAsStringAsync());

            var response = await toniClient.DeleteAsync($"/api/topic/{postTopic.Id}");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }
    }
}
