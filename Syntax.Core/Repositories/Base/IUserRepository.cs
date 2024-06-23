using Syntax.Core.Models;

namespace Syntax.Core.Repositories.Base
{
    public interface IUserRepository
    {
        Task<UserAccount?> GetUserById(string id);
        Task<Blob?> GetUserProfilePictureAsync(string userId);
        Task SetUserProfilePictureAsync(string userId, Blob blob);
        Task SaveChangesAsync();
    }
}