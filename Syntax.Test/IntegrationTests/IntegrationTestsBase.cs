using Microsoft.Extensions.DependencyInjection;
using Syntax.API.Requests;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Syntax.Tests.IntegrationTests
{
    public class IntegrationTestsBase
    {
        private TestApplicationFactory Factory;
        protected string TimoTestUsername = "TimoTest";
        protected string ToniTestUsername = "ToniTest";

        public IntegrationTestsBase()
        {
            Factory = new TestApplicationFactory();
        }

        protected T DeserializeWithOptions<T>(string json) where T : class
        {
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            });
        }

        protected async Task<HttpClient> CreateClientAndAuthenticate(string username)
        {
            var client = Factory.CreateClient();

            await client.PostAsync("/api/authentication/login", new SigninRequest
            {
                Username = username,
                Password = "Testi123"
            }.ToJsonStringContent());

            return client;
        }
    }
}