using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syntax.Core.Data;
using Syntax.Core.Dtos;
using Syntax.Core.Models;
using Syntax.Core.Services.Base;
using System.ComponentModel.DataAnnotations;

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

        public UserController(UserManager<UserAccount> userManager, IFileService fileService, ApplicationDbContext applicationDbContext)
        {
            _userManager = userManager;
            _fileService = fileService;
            _applicationDbContext = applicationDbContext;
        }

        [HttpPost("settings")]
        public async Task<IActionResult> PostUserSettings([FromForm] UserInformationRequest request)
        {
            var user = await _userManager.GetUserAsync(User);

            if(user == null)
                return Unauthorized();

            try
            {
                if(request.File != null)                    
                    user.UserSettings.ProfilePicture = await _fileService.UploadFileAsync(request.File, user);
                
                // TODO: Topic and Comment flag                
                await _userManager.SetUserNameAsync(user, request.DisplayName);
            }            
            catch
            {
                throw new Exception("Error saving image");
            }

            await _applicationDbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("settings/{userId}")]
        public async Task<IActionResult> GetUserSettings(Guid userId)
        {
            UserAccount? user = await _applicationDbContext.Users
                .Include(u => u.UserComments)
                .Include(u => u.UserTopics)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId.ToString());

            return user != null ? new JsonResult(new UserInformationDto
            {
                DisplayName = user.UserSettings.DisplayName,
                ShowComments = user.UserSettings.ShowComments,
                ShowTopics = user.UserSettings.ShowTopics,
                ProfilePicture = user.UserSettings.ProfilePicture?.Path
            }) : StatusCode(404);
        }

        [HttpGet("details/{userId}")]
        public async Task<IActionResult> GetUserDetails(Guid userId)
        {
            // TODO: Service
            UserAccount? user = await _applicationDbContext.Users
                .Include(u => u.UserComments)
                .Include(u => u.UserTopics)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId.ToString());

            return user != null ? new JsonResult(new UserDetailsDto
            {
                User = new UserDto(user),
                Comments = user.UserComments.Select(c => new CommentDto(c)),
                Topics = user.UserTopics.Select(t => new TopicDto(t))
            }) : StatusCode(404);
        }
    }

    public class UserInformationRequest
    {
        [Required]
        public string DisplayName { get; set; } = null!;

        [Required]
        public bool ShowTopics { get; set; }
        
        [Required]
        public bool ShowComments { get; set; }

        public IFormFile? File { get; set; }
    }

    public class UserInformationDto
    {
        public string DisplayName { get; set; } = null!;

        public bool ShowTopics { get; set; }

        public bool ShowComments { get; set; }

        public string? ProfilePicture { get; set; }
    }

    public class UserDetailsDto
    {
        public UserDto User { get; set; } = new UserDto();
        public IEnumerable<CommentDto> Comments { get; set; } = [];
        public IEnumerable<TopicDto> Topics { get; set; } = [];
    }
}
