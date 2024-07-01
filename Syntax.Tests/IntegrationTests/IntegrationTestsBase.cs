using Syntax.API.Requests;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Syntax.Tests.IntegrationTests
{
    internal class IntegrationTestsBase
    {
        protected TestApplicationFactoryStartup Factory;
        protected HttpClient Client;
        protected string Username = "TimoTest";
        protected string Email = "TimoTest";

        protected IntegrationTestsBase()
        {
            Factory = new TestApplicationFactoryStartup();
            Client = Factory.CreateClient();
        }

        protected T DeserializeWithOptions<T>(string json) where T : class
        {
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            });
        }

        protected async Task Authenticate()
        {
            await Client.PostAsync("/api/authentication/login", new SigninRequest
            {
                Username = "TimoTest",
                Password = "Testi123"
            }.ToJsonStringContent());
        }
    }
}