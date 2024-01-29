using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Syntax.Core.Models;
using Syntax.Core.Repositories;
using Syntax.Core.Repositories.Base;
using Syntax.Core.Services.Base;
using System.Security.Claims;

namespace Syntax.Core.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<UserAccount> _userManager;
        private readonly SignInManager<UserAccount> _signInManager;
        private readonly IUserStore<UserAccount> _userStore;
        private readonly UnitOfWork _unitOfWork;
        private readonly IUserEmailStore<UserAccount> _emailStore;

        public UserService(UserManager<UserAccount> userManager, SignInManager<UserAccount> signInManager, IUserStore<UserAccount> userStore, UnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userStore = userStore;
            _unitOfWork = unitOfWork;
            _emailStore = GetEmailStore();
        }

        public async Task<SignInResult> SignInAsync(string username, string password)
        {            
            var result = await _signInManager.PasswordSignInAsync(username, password, false, false);

            return result;
        }

        public async Task GenerateSignInCookie(HttpContext httpContext, UserAccount user)
        {
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("Email", user.Email!),
            };

            var authProperties = new AuthenticationProperties()
            {
                IsPersistent = true,
                AllowRefresh = true,
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
        }

        public async Task<IdentityResult> Register(string email, string password, string username)
        {
            var user = Activator.CreateInstance<UserAccount>();
            user.UserName = email;
            user.Email = email;
            user.UserName = username;

            return await _userManager.CreateAsync(user, password);
        }

        public async Task<bool> AssignToRole(UserAccount user, string role)
        {
            if (await _userManager.IsInRoleAsync(user, role))
                return true;

            var result = await _userManager.AddToRoleAsync(user, role);

            return result.Succeeded;
        }

        public async Task<IdentityResult> CreateUser(UserAccount user, string username, string password, string email)
        {
            await _userStore.SetUserNameAsync(user, username, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, email, CancellationToken.None);
            return await _userManager.CreateAsync(user, password);
        }

        private IUserEmailStore<UserAccount> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
                throw new NotSupportedException("The default UI requires a user store with email support.");

            return (IUserEmailStore<UserAccount>)_userStore;
        }
    }
}
