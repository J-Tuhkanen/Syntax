using Microsoft.AspNetCore.Mvc;
using Syntax.Core.Models;
using Syntax.Controllers.Requests.Post;
using Syntax.Core.Services.Base;
using Syntax.Core.Wrappers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

namespace Syntax.Controllers
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

        [HttpPost]
        public async Task<IActionResult> GetPosts([FromBody] GetPostsRequest request)
        {
            IEnumerable<Post> posts = await _postService.GetPostsAsync(request.ExcludedPosts, request.Amount);
            IEnumerable<PostWrapper> postWrappers = posts.Select(p => new PostWrapper(p));

            return Content(JsonSerializer.Serialize(postWrappers.OrderByDescending(pw => pw.Timestamp.Ticks)));
        }
    }
}
