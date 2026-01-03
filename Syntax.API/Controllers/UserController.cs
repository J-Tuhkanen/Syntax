using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syntax.Core.Data;
using Syntax.Core.Dtos;
using Syntax.Core.Models;
using Syntax.Core.Services.Base;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

namespace Syntax.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly UserManager<UserAccount> _userManager;
        private readonly IFileService _fileService;
        private readonly ApplicationDbContext _applicationDbContext;

        public UserController(
            UserManager<UserAccount> userManager, 
            IFileService fileService, 
            ApplicationDbContext applicationDbContext)
        {
            _userManager = userManager;
            _fileService = fileService;
            _applicationDbContext = applicationDbContext;
        }

        [HttpPost("settings")]
        public async Task<IActionResult> PostUserSettings([FromForm] UserSettingsRequest request)
        {
            var user = await _userManager.GetUserAsync(User);

            if(user == null)
                return Unauthorized();

            try
            {                
                await _applicationDbContext.UserSettings.Include(s => s.ProfilePicture).LoadAsync();

                user.UserSettings.ShowTopics = request.ShowTopics;
                user.UserSettings.ShowComments = request.ShowComments;
                
                if(request.File != null)
                {                    
                    user.UserSettings.ProfilePicture = await _fileService.UploadFileAsync(request.File, user);
                }

                await _userManager.SetUserNameAsync(user, request.DisplayName);
            }            
            catch
            {
                throw new Exception("Error saving image");
            }

            await _applicationDbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("details/{userId}")]
        public async Task<IActionResult> GetUserDetails(Guid userId)
        {
            UserAccount? user = await _applicationDbContext.Users
                .Include(u => u.UserComments)
                .Include(u => u.UserTopics)
                .Include(u => u.UserSettings).ThenInclude(s => s.ProfilePicture)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId.ToString());

            var userDetails = new UserDetailsDto(
                user.Email, 
                user.JoinedDate,
                new UserSettingsDto(
                    user.UserSettings.DisplayName,
                    user.UserSettings.ShowTopics,
                    user.UserSettings.ShowComments,
                    user.UserSettings.ProfilePicture?.Path),
                new UserActivityDto(
                    user.UserComments.Select(c => new CommentDto(c)),
                    user.UserTopics.Select(t => new TopicDto(t))));

            return user != null ? new JsonResult(userDetails) : StatusCode(404);
        }

        public class UserSettingsRequest
        {
            [Required]
            public string DisplayName { get; set; } = null!;

            [Required]
            public bool ShowTopics { get; set; }

            [Required]
            public bool ShowComments { get; set; }

            public IFormFile? File { get; set; }
        }
    }
}
