using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Syntax.API.Requests;
using Syntax.Core.Models;
using Syntax.Core.Services.Base;

namespace Syntax.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;
        private readonly UserManager<UserAccount> _userManager;

        public CommentController(ICommentService commentService, UserManager<UserAccount> userManager)
        {
            _commentService = commentService;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> PostComment([FromBody] CommentRequest commentRequest)
        {
            var user = await _userManager.GetUserAsync(User);
            var comment = await _commentService.CreateCommentAsync(commentRequest.TopicId, commentRequest.Content, user);

            return new JsonResult(comment);
        }

        [HttpGet("{topicId}")]
        public async Task<IActionResult> GetComments(Guid topicId)
        {
            var comments = await _commentService.GetCommentsAsync(topicId, new List<Guid>(), 5);

            return new JsonResult(comments);
        }

        [HttpDelete("{commentId}")]
        public async Task<IActionResult> DeleteComment(Guid commentId)
        {
            var user = await _userManager.GetUserAsync(User);
            var comment = await _commentService.DeleteCommentAsync(commentId, user);

            return comment != null ? Ok() : StatusCode(403);
        }
    }
}
