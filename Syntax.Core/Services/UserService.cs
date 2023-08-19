using Syntax.Core.Models;
using Syntax.Core.Repositories.Base;
using Syntax.Core.Services.Base;

namespace Syntax.Core.Services
{
    public class UserService : IUserService
    {
        private IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task SetUserProfilePictureAsync(string userId, Blob blob)
        {
            await _userRepository.SetUserProfilePictureAsync(userId, blob);
            await _userRepository.SaveChangesAsync();
        }

        public async Task<Blob> GetUserProfilePictureAsync(string userId)
            => await _userRepository.GetUserProfilePictureAsync(userId);

        public async Task<UserAccount> GetUserByIdAsync(string id)
            => await _userRepository.GetUserById(id);
    }
}
