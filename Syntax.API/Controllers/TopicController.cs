﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS.Core;
using Syntax.API.Requests;
using Syntax.Core.Dtos;
using Syntax.Core.Models;
using Syntax.Core.Services.Base;
using Syntax.Core.Wrappers;
using System.Text.Json;

namespace Syntax.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TopicController : ControllerBase
    {
        private readonly ITopicService _topicService;
        private readonly UserManager<UserAccount> _userManager;

        public TopicController(ITopicService topicService, UserManager<UserAccount> userManager)
        {
            _topicService = topicService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetTopicsAsync()
        {
            var topics = await _topicService.GetTopicsAsync(new List<Guid>(), 100);
            return new JsonResult(topics.Select(t => new TopicDto(t)));
        }

        [HttpGet("{topicId}")]
        public async Task<IActionResult> GetTopicAsync(Guid topicId)
        {
            Topic? topic = await _topicService.GetTopicAsync(topicId);

            return topic != null ? new JsonResult(new TopicDto(topic)) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> PostTopicAsync(TopicRequest request)
        {
            var user = await _userManager.GetUserAsync(User);
            var topic = await _topicService.CreateTopicAsync(request.Title, request.Body, user);

            return new JsonResult(new TopicDto(topic));
        }

        [HttpDelete("{topicId}")]
        public async Task<IActionResult> DeleteTopicAsync(Guid topicId)
        {
            return await _topicService.DeleteTopicAsync(topicId, await _userManager.GetUserAsync(User)) != null ? Ok() : StatusCode(403);
        }

        [HttpPut("{topicId}")]
        public async Task<IActionResult> UpdateTopicAsync()
        {
            throw new NotImplementedException();
        }
    }
}
