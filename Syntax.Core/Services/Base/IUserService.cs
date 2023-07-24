using Syntax.Core.Models;

namespace Syntax.Core.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserAccount> GetUserAccountAsync(string id);
        Task<Blob> GetUserProfilePictureAsync(string userId);
        Task SetUserProfilePictureAsync(string userId, Blob blob);
    }
}