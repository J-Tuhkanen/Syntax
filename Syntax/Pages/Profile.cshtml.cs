using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Syntax.Core.Data;
using Syntax.Core.Models;
using Syntax.Core.Models.Base;
using Syntax.Core.Services;
using Syntax.Core.Services.Base;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Syntax.Pages
{
    public class ProfileModel : PageModel
    {
        private UserManager<UserAccount> _userManager;
        private ApplicationDbContext _dbContext;
        private IPostService _postService;
        private ICommentService _commentService;

        public UserAccount ViewedUser { get; private set; }

        public Blob ProfilePictureBlob { get; private set; }

        public IEnumerable<Post> UserPosts { get; private set; }
        public IEnumerable<Comment> UserComments { get; private set; }

        public IEnumerable<IUserActivity> UserActivity { get; private set; }
        public bool ShowEditButton { get; private set; }

        public ProfileModel(
            UserManager<UserAccount> userManager, 
            ApplicationDbContext dbContext, 
            IPostService postService,
            ICommentService commentService)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _postService = postService;
            _commentService = commentService;
        }

        public async Task OnGetAsync(string id)
        {
            ViewedUser = await _userManager.FindByIdAsync(id);
            ProfilePictureBlob = await _dbContext.Blobs.FirstOrDefaultAsync(b => b.Id == ViewedUser.ProfilePictureFileId);

            UserPosts = await _postService.GetPostsByUserAsync(id, new List<string>(), 5);
            UserComments = await _commentService.GetCommentsByUserAsync(id, new List<string>(), 5);

            UserActivity = ((IEnumerable<IUserActivity>)UserPosts).Concat(UserComments).OrderBy(a => a.Timestamp);
        }
    }
}
