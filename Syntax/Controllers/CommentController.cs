using Microsoft.AspNetCore.Mvc;
using Syntax.Controllers.Requests.Comment;
using Syntax.Core.Services.Base;
using System.Threading.Tasks;

namespace Syntax.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService) 
        {
            _commentService = commentService;
        }

        [HttpPost]
        public async Task<IActionResult> GetComments([FromBody] GetCommentRequest request)
        {
            return new ObjectResult(await _commentService.GetCommentsAsync(request.PostId, request.ListOfExcludedComments, request.Amount));
        }

    }
}
