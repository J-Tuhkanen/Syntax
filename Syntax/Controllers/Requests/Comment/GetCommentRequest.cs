using System.Collections.Generic;

namespace Syntax.Controllers.Requests.Comment
{
    public class GetCommentRequest
    {
        public string PostId { get; set; }

        public IEnumerable<string> ListOfExcludedComments { get; set; }

        public int Amount { get; set; }
    }
}
