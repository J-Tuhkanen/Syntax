using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Syntax.Data;
using Syntax.Models;
using Syntax.Services;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Syntax.Pages
{
    public class ProfileModel : PageModel
    {
        private UserManager<UserAccount> _userManager;
        private ApplicationDbContext _dbContext;
        private IPostService _postService;

        public UserAccount ViewedUser { get; private set; }

        public Blob ProfilePictureBlob { get; private set; }

        public IEnumerable<Post> UserPosts { get; private set; }

        public bool ShowEditButton { get; private set; }

        public ProfileModel(UserManager<UserAccount> userManager, ApplicationDbContext dbContext, IPostService postService)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _postService = postService;
        }

        public async Task OnGetAsync(string id)
        {
            ViewedUser = await _userManager.FindByIdAsync(id);
            ProfilePictureBlob = await _dbContext.Blobs.FirstOrDefaultAsync(b => b.Id == ViewedUser.ProfilePictureFileId);
            UserPosts = await _postService.GetPostsByUserAsync(id);

        }
    }
}
