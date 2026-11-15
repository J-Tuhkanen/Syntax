using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Syntax.Core.Models;

namespace Syntax.Core.Services.Base
{
    public interface IUserService
    {
        Task GenerateSignInCookie(HttpContext httpContext, UserAccount user);
        Task<IdentityResult> Register(string email, string password, string username, string displayname);
        Task<SignInResult> SignInAsync(string email, string password);
        Task<bool> AssignToRole(UserAccount user, string role);
        Task<IdentityResult> CreateUser(UserAccount user, string username, string password, string email);
        Task<UserAccount?> GetUserById(string id);
    }
}