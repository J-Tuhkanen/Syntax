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

        [HttpPost]
        public async Task<IActionResult> UploadUserSettings([FromForm] UserInformationRequestDto request)
        {
            var user = await _userManager.GetUserAsync(User);

            if(user == null)
            {
                return Unauthorized();
            }

            try
            {
                if(request.File != null)                    
                    user.ProfilePictureBlob = await _fileService.UploadFileAsync(request.File, user);
            }            
            catch
            {
                throw new Exception("Error saving image");
            }


            await _applicationDbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("avatar/{userId}")]
        public async Task<IActionResult> GetUseProfilePicture(Guid userId)
        {
            // TODO: Service
            UserAccount? user = await _applicationDbContext.Users
                .Include(u => u.ProfilePictureBlob)
                .FirstOrDefaultAsync(u => u.Id == userId.ToString());

            if(user == null)
            {
                return NotFound();
            }

            if(user.ProfilePictureBlob == null)
            {
                return Ok();
            }

            var image = System.IO.File.OpenRead(Path.Combine($"Uploads/{user.UserName?.ToLower()}", user.ProfilePictureBlob.Path));
            
            return File(image, $"image/{new FileInfo(user.ProfilePictureBlob.Path).Extension}");
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

            return user != null ? new JsonResult(new { 
                User = new UserDto(user),
                Comments = user.UserComments.Select(c => new CommentDto(c)),
                Topics = user.UserTopics.Select(t => new TopicDto(t))
            }) : StatusCode(404);
        }
    }

    public class UserInformationRequestDto
    {
        public IFormFile? File { get; set; }
        
        [Required]
        public string UserName { get; set; }
        
        [Required]
        public string Email { get; set; }
     
        [Required]
        public string Description { get; set; }
    }
}
