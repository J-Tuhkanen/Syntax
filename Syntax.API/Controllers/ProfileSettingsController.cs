using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Syntax.Core.Helpers;
using Syntax.Core.Models;
using Syntax.Core.Services.Base;
using System.ComponentModel.DataAnnotations;

namespace Syntax.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileSettingsController : Controller
    {
        private readonly UserManager<UserAccount> _userManager;
        private readonly IFileService _fileService;

        public ProfileSettingsController(UserManager<UserAccount> userManager, IFileService fileService)
        {
            _userManager = userManager;
            _fileService = fileService;
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage([FromForm] UserInformationRequestDto request)
        {
            var user = await _userManager.GetUserAsync(User);

            if(user == null)
            {
                return Unauthorized();
            }

            try
            {
                await _fileService.UploadFileAsync(request.File, user);

                // Your logic to handle the file upload
                return Ok();
            }
            catch
            {
                throw new Exception("Error saving image");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserInformation()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Your logic to return user information
            return Ok(user);
        }

        private async Task<ImageFormat> GetFileFormat(IFormFile file)
        {
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);

                return FileHelper.GetImageFormatFromBytes(stream.ToArray());
            }
        }
    }

    public class UserInformationRequestDto
    {
        [Required]
        public IFormFile File { get; set; }
        //public string UserName { get; set; }
        //public string Email { get; set; }
        //public string Description { get; set; }
        //public string AccountEnabled { get; set; }
    }

    public class UserInformationResponseDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public bool AccountEnabled { get; set; }

        public UserInformationResponseDto(string username, string email, string description, bool accountEnabled)
        {
            Username = username;
            Email = email;
            Description = description;
            AccountEnabled = accountEnabled;
        }
    }
}
