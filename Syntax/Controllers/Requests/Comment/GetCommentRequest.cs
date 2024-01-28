using System;
using System.Collections.Generic;

namespace Syntax.Controllers.Requests.Comment
{
    public class GetCommentRequest
    {
        public Guid PostId { get; set; }

        public IEnumerable<Guid> ListOfExcludedComments { get; set; }

        public int Amount { get; set; }
    }
}
