using System.Text;
using System.Text.Json;

namespace Syntax.Tests
{
    public static class TestExtensions
    {
        public static StringContent ToJsonStringContent(this object obj)
        {
            return new(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json");
        }
    }
}
