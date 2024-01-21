using Microsoft.AspNetCore.Mvc;
using Syntax.Core.Models;
using Syntax.Core.Services.Base;
using Syntax.Core.Wrappers;
using System.Text.Json;

namespace Syntax.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet("{topicId}")]
        public async Task<IActionResult> GetTopicAsync(string topicId)
        {
            Post post = await _postService.GetTopicAsync(topicId);

            return Content(JsonSerializer.Serialize(new PostWrapper(post)));
        }
    }
}
