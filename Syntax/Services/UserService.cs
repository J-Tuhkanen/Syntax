using Microsoft.EntityFrameworkCore;
using Syntax.Data;
using Syntax.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Syntax.Services
{
    public class UserService
    {
        private ApplicationDbContext _appDbContext;

        public UserService(ApplicationDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task SetUserProfilePictureAsync(string userId, Blob blob)
        {
            var user = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        
            if(user != null)
            {
                user.ProfilePictureFileId = blob.Id;

                await _appDbContext.SaveChangesAsync();
            }
        }
    }
}
