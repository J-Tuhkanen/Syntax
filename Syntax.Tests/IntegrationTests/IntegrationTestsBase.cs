using System.Text.Json;
using System.Text.Json.Serialization;

namespace Syntax.Tests.IntegrationTests
{
    internal class IntegrationTestsBase
    {

        public T DeserializeWithOptions<T>(string json) where T : class
        {
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            });
        }
    }
}