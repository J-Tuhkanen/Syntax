using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syntax.Core.Services.Base;
using Syntax.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace Syntax.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly UserManager<UserAccount> _userManager;

        public CommentController(ICommentService commentService, UserManager<UserAccount> userManager)
        {
            _commentService = commentService;
            _userManager = userManager;
        }

        [HttpGet("{topicId}")]
        public async Task<IActionResult> GetCommentsAsync(Guid topicId)
        {
            var comments = await _commentService.GetCommentsAsync(topicId);
            return new JsonResult(comments);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCommentAsync([FromBody] Comment comment, Guid topicId)
        {
            var user = await _userManager.GetUserAsync(User);
            var newComment = await _commentService.CreateCommentAsync(topicId, comment.Content, user.Id);
            return new JsonResult(newComment);
        }

        [HttpDelete("{commentId}")]
        public async Task<IActionResult> DeleteCommentAsync(Guid commentId)
        {
            await _commentService.DeleteCommentAsync(commentId);
            return Ok();
        }
    }
}
