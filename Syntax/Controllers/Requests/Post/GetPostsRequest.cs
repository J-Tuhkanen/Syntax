using System.Collections.Generic;

namespace Syntax.Controllers.Requests.Post
{
    public class GetPostsRequest
    {
        public IEnumerable<string> ExcludedPosts { get; set; }
        public int Amount { get; set; }
    }
}
