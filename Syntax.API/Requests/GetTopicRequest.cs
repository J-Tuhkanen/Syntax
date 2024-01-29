namespace Syntax.API.Requests
{
    public class GetTopicRequest
    {
        public IEnumerable<Guid> ExcludedTopics { get; set; } = Enumerable.Empty<Guid>();
        public int Amount { get; set; }
    }
}
