using System;
using System.Collections.Generic;

namespace Syntax.Controllers.Requests.Post
{
    public class GetPostsRequest
    {
        public IEnumerable<Guid> ExcludedPosts { get; set; }
        public int Amount { get; set; }
    }
}
