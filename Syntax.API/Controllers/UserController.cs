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
        public async Task<IActionResult> PostUserSettings([FromForm] UserInformationDto request)
        {
            var user = await _userManager.GetUserAsync(User);

            if(user == null)
                return Unauthorized();

            try
            {
                if(request.File != null)                    
                    user.ProfilePictureBlob = await _fileService.UploadFileAsync(request.File, user);
                
                // TODO: Topic and Comment flag                
                await _userManager.SetUserNameAsync(user, request.UserName);
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
            var user = await _applicationDbContext.Users
                .Include(u => u.UserComments)
                .Include(u => u.UserTopics)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId.ToString());

            return user != null ? new JsonResult(new UserInformationDto
            {
                UserName = user.UserName,
                ShowComments = true,
                ShowTopics = true,
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

            return user != null ? new JsonResult(new UserDetailsResponse
            {
                User = new UserDto(user),
                Comments = user.UserComments.Select(c => new CommentDto(c)),
                Topics = user.UserTopics.Select(t => new TopicDto(t))
            }) : StatusCode(404);
        }
    }

    public class UserAvatarRequestDto
    {
        public IFormFile? File { get; set; }
    }

    public class UserInformationDto
    {
        [Required]
        public string UserName { get; set; } = null!;

        [Required]
        public bool ShowTopics { get; set; }

        [Required]
        public bool ShowComments { get; set; }

        public IFormFile? File { get; set; } = null;
    }

    public class UserDetailsResponse
    {
        public UserDto User { get; set; }
        public IEnumerable<CommentDto> Comments { get; set; }
        public IEnumerable<TopicDto> Topics { get; set; }
    }
}
