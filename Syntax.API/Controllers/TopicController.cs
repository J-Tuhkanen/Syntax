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
    public class TopicController : ControllerBase
    {
        private readonly IUserActivityService _userActivityService;
        private readonly UserManager<UserAccount> _userManager;

        public TopicController(IUserActivityService userActivityService, UserManager<UserAccount> userManager)
        {
            _userActivityService = userActivityService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetTopicsAsync()
        {
            var topics = await _userActivityService.GetTopicsAsync(new List<Guid>(), 100);
            return new JsonResult(topics);
        }

        [HttpGet("{topicId}")]
        public async Task<IActionResult> GetTopicAsync(Guid topicId)
        {
            Topic? topic = await _userActivityService.GetTopicAsync(topicId);

            return topic != null ? new JsonResult(topic) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> PostTopicAsync([FromBody] TopicRequest request)
        {
            var user = await _userManager.GetUserAsync(User);
            var topic = await _userActivityService.CreateTopicAsync(request.Title, request.Body, user);

            return new JsonResult(topic);
        }

        [HttpDelete("{topicId}")]
        public async Task<IActionResult> DeleteTopicAsync(Guid topicId)
        {
            return await _userActivityService.DeleteTopicAsync(topicId) != null ? Ok() : NotFound();
        }

        [HttpPut("{topicId}")]
        public async Task<IActionResult> UpdateTopicAsync(Guid topicId, [FromBody] TopicRequest request)
        {
            var user = await _userManager.GetUserAsync(User);
            var topic = await _userActivityService.UpdateTopicAsync(topicId, request.Title, request.Body, user);
            
            if (topic?.User.Id != user.Id)
            {
                return Forbid();
            }

            return new JsonResult(topic);
        }
    }
}
