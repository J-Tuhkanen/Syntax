using Syntax.API.Requests;
using System.Net;

namespace Syntax.Tests.IntegrationTests
{
    internal class TopicIntegrationTests : IntegrationTestsBase
    {
        [Test]
        public async Task CreateTopicAndAddCommentToIt()
        {
            await Authenticate();

            var requestContent = new TopicRequest
            {
                Body = "TestBody",
                Title = "TestTitle"
            };

            var topicResponse = await Client.PostAsync("/api/topic", requestContent.ToJsonStringContent());
            var contentString = await topicResponse.Content.ReadAsStringAsync();

            if (topicResponse.IsSuccessStatusCode == false)
            {
                Assert.Fail(contentString);
            }

            var topic = DeserializeWithOptions<Topic>(contentString);

            var commentResponse = await Client.PostAsync("/api/comment", new CommentRequest
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
            await Authenticate();

            var requestContent = new TopicRequest
            {
                Body = "TestBody",
                Title = "TestTitle"
            };

            var postTopicResponse = await Client.PostAsync("/api/topic", requestContent.ToJsonStringContent());
            var postTopic = DeserializeWithOptions<Topic>(await postTopicResponse.Content.ReadAsStringAsync());

            await Client.PostAsync("/api/comment", new CommentRequest
            {
                Content = "TestComment",
                TopicId = postTopic.Id
            }.ToJsonStringContent());

            var getTopicResponse = await Client.GetAsync($"/api/topic/{postTopic.Id}");
            var getTopic = DeserializeWithOptions<Topic>(await getTopicResponse.Content.ReadAsStringAsync());

            Assert.That(getTopic.Title, Is.EqualTo(postTopic.Title));
            Assert.That(getTopic.Body, Is.EqualTo(postTopic.Body));
            Assert.True(getTopic.Comments.Count == 1);
            Assert.That(getTopic.User.UserName, Is.EqualTo(TestApplicationFactoryStartup.User.UserName));
            Assert.That(getTopic.User.Email, Is.EqualTo(TestApplicationFactoryStartup.User.Email));
        }

        [Test]
        public async Task CreateTopicAndDelete()
        {
            await Authenticate();

            var requestContent = new TopicRequest
            {
                Body = "TestBody",
                Title = "TestTitle"
            };

            var postTopicResponse = await Client.PostAsync("/api/topic", requestContent.ToJsonStringContent());
            var postTopic = DeserializeWithOptions<Topic>(await postTopicResponse.Content.ReadAsStringAsync());

            await Client.DeleteAsync($"/api/topic/{postTopic.Id}");

            var response = await Client.GetAsync($"/api/topic/{postTopic.Id}");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task CreateTopicAndUpdateItsTitleAndContent()
        {
            await Authenticate();

            var requestContent = new TopicRequest
            {
                Body = "TestBody",
                Title = "TestTitle"
            };

            var postTopicResponse = await Client.PostAsync("/api/topic", requestContent.ToJsonStringContent());
            var postTopic = DeserializeWithOptions<Topic>(await postTopicResponse.Content.ReadAsStringAsync());
            var updateRequest = new TopicRequest
            {
                Body = "UpdatedTestBody",
                Title = "UpdatedTestTitle"
            };
            
            await Client.PutAsync($"/api/topic/{postTopic.Id}", updateRequest.ToJsonStringContent());
            var response = await Client.GetAsync($"/api/topic/{postTopic.Id}");

            var topic = DeserializeWithOptions<Topic>(await response.Content.ReadAsStringAsync());

            Assert.That(topic.Body, Is.EqualTo("UpdatedTestBody"));
            Assert.That(topic.Title, Is.EqualTo("UpdatedTestTitle"));
        }

        // Delete comment
        // Update comment
    }
}
