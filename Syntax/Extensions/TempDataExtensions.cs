namespace Syntax.Extensions
{
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using System.Text.Json;

    /// <summary>
    /// Extension methods to add feature to save objects into temp data.
    /// </summary>
    public static class TempDataExtensions
    {
        public static void Put<T>(this ITempDataDictionary tempData, string key, T value) where T : class 
            => tempData[key] = JsonSerializer.Serialize(value);        

        public static T Get<T>(this ITempDataDictionary tempData, string key) where T : class
        {
            tempData.TryGetValue(key, out object o);

            return o == null 
                ? null 
                : JsonSerializer.Deserialize<T>((string)o);
        }
    }
}
