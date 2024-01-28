using Microsoft.AspNetCore.Mvc;
using Syntax.Core.Models;
using Syntax.Core.Services.Base;
using Syntax.Core.Wrappers;
using System.Text.Json;

namespace Syntax.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TopicController : ControllerBase
    {
        private readonly ITopicService _topicService;

        public TopicController(ITopicService topicService)
        {
            _topicService = topicService;
        }

        [HttpGet("{topicId}")]
        public async Task<IActionResult> GetTopicAsync(Guid topicId)
        {
            Topic topic = await _topicService.GetTopicAsync(topicId);

            return Content(JsonSerializer.Serialize(new TopicWrapper(topic)));
        }

        [HttpDelete("{topicId}")]
        public async Task<IActionResult> DeleteTopicAsync(Guid topicId)
        {
            var topic = await _topicService.DeleteTopicAsync(topicId);
            return Ok();
        }

        [HttpPut("{topicId}")]
        public async Task<IActionResult> UpdateTopicAsync()
        {
            return Ok();
        }
    }
}
