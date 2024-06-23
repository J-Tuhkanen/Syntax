using Microsoft.EntityFrameworkCore;
using Syntax.Core.Data;
using Syntax.Core.Models;
using Syntax.Core.Repositories.Base;

namespace Syntax.Core.Repositories
{
    internal class UserRepository : RepositoryBase, IUserRepository
    {
        internal UserRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
        }

        public async Task SetUserProfilePictureAsync(string userId, Blob blob)
        {
            var user = await applicationDbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user != null)
            {
                user.ProfilePictureFileId = blob.Id;
            }
        }

        public async Task<Blob?> GetUserProfilePictureAsync(string userId)
        {
            var user = await applicationDbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

            return user?.ProfilePictureFileId != null
                ? await applicationDbContext.Blobs.FirstOrDefaultAsync(b => b.Id == user.ProfilePictureFileId)
                : null;
        }

        public async Task<UserAccount?> GetUserById(string id)
            => await applicationDbContext.Users.FirstOrDefaultAsync(u => u.Id == id);

    }
}
