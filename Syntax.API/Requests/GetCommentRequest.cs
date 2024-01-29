namespace Syntax.API.Requests
{
    public class GetCommentRequest
    {
        public Guid PostId { get; set; }

        public IEnumerable<Guid> ListOfExcludedComments { get; set; } = Enumerable.Empty<Guid>();

        public int Amount { get; set; }
    }
}
