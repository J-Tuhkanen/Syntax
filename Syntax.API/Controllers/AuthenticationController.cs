using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syntax.API.Requests;
using Syntax.Core.Models;
using Syntax.Core.Services.Base;

namespace Syntax.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : Controller
    {
        private readonly IUserService _authService;
        private readonly UserManager<UserAccount> _userManager;

        public AuthenticationController(IUserService authService, UserManager<UserAccount> userManager)
        {
            _authService = authService;
            _userManager = userManager;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync([FromBody] SigninRequest request)
        {
            if (ModelState.IsValid == false)
                return BadRequest();

            var loginResult = await _authService.SignInAsync(request.Username, request.Password);

            if (loginResult.Succeeded)
            {
                try
                {
                    UserAccount user = await _userManager.Users.FirstAsync(u => u.UserName == request.Username);
                    await _authService.GenerateSignInCookie(HttpContext, user);

                    return new JsonResult(new ApplicationUserRecord(user));
                }
                catch
                {
                    return new StatusCodeResult(500);
                }
            }

            return Unauthorized();
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync([FromBody] SignupRequest request)
        {
            if (ModelState.IsValid == false)
                return BadRequest();

            var result = await _authService.Register(request.Email, request.Password, request.Username);

            if (result.Succeeded)
                return Ok();

            return BadRequest();
        }

        [Authorize]
        [HttpPost("Logout")]
        public async Task<IActionResult> LogoutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }

        [HttpGet("Session")]
        public async Task<IActionResult> GetCurrentSession()
        {
            UserAccount user = await _userManager.GetUserAsync(User);

            return user == null ? NoContent() : new JsonResult(new ApplicationUserRecord(user));
        }
    }

    public record class ApplicationUserRecord
    {
        public string Id { get; }
        public string DisplayName { get; }

        public ApplicationUserRecord(UserAccount user)
        {
            Id = user.Id;
            DisplayName = user.UserName;
        }
    }
}
