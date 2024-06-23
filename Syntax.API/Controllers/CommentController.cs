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
        public async Task<IActionResult> PostTopic(CommentRequest commentRequest)
        {
            var user = await _userManager.GetUserAsync(User);
            var comment = await _commentService.CreateCommentAsync(commentRequest.TopicId, commentRequest.Content, user.Id);

            return new JsonResult(comment);
        }
    }
}
