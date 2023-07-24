using Microsoft.EntityFrameworkCore;
using Syntax.Core.Data;
using Syntax.Core.Models;
using Syntax.Core.Services.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace Syntax.Core.Services
{
    public class UserService : IUserService
    {
        private ApplicationDbContext _appDbContext;

        public UserService(ApplicationDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task SetUserProfilePictureAsync(string userId, Blob blob)
        {
            var user = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user != null)
            {
                user.ProfilePictureFileId = blob.Id;

                await _appDbContext.SaveChangesAsync();
            }
        }

        public async Task<Blob> GetUserProfilePictureAsync(string userId)
        {
            var user = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

            return user.ProfilePictureFileId != null
                ? await _appDbContext.Blobs.FirstOrDefaultAsync(b => b.Id == user.ProfilePictureFileId)
                : null;
        }

        public async Task<UserAccount> GetUserAccountAsync(string id)
            => await _appDbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
    }
}
