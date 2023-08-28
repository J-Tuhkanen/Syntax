using Microsoft.AspNetCore.Identity;
using Syntax.Core.Models;

namespace Syntax.Core.Services.Base
{
    public interface IUserService
    {
        Task<UserAccount> GetUserByIdAsync(string id);
        Task<Blob> GetUserProfilePictureAsync(string userId);
        Task SetUserProfilePictureAsync(string userId, Blob blob);
        Task<IdentityResult> CreateUser(UserAccount user, string username, string password, string email);
    }
}