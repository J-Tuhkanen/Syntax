using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Syntax.API.Requests;
using Syntax.Core.Dtos;
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

            return comment != null ? new JsonResult(new CommentDto(comment)) : throw new Exception("Something went wrong with creating a new comment");
        }

        [HttpGet("{topicId}")]
        public async Task<IActionResult> GetComments(Guid topicId)
        {
            var comments = await _commentService.GetCommentsAsync(topicId, new List<Guid>(), 5);

            return new JsonResult(comments.Select(c => new CommentDto(c)));
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
