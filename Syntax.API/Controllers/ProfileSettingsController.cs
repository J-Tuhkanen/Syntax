using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Syntax.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace Syntax.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileSettingsController : Controller
    {
        private readonly UserManager<UserAccount> _userManager;

        public ProfileSettingsController(UserManager<UserAccount> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage([FromForm] UserInformationRequestDto request)
        {
            if (request.File == null)
            {
                return BadRequest("The File field is required.");
            }

            // Your logic to handle the file upload
            return Ok();
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
    }

    public class UserInformationRequestDto
    {
        public IFormFile File { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public string AccountEnabled { get; set; }
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
