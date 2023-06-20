using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Syntax.Data;
using Syntax.Models;
using Syntax.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Syntax.Pages
{
    public class PostModel : PageModel
    {
        private IPostService _postService;
        private ICommentService _commentService;
        private UserManager<UserAccount> _userManager;
        private ApplicationDbContext _appDbContext;

        public PostModel(
            IPostService postService, 
            ICommentService commentService, 
            UserManager<UserAccount> userManager,
            ApplicationDbContext appDbContext)
        {
            _postService = postService;
            _commentService = commentService;
            _userManager = userManager;
            _appDbContext = appDbContext;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public string Comment { get; set; }
        }

        public Post Post { get; private set; }
        public IEnumerable<Comment> Comments { get; private set; }
        public UserAccount PostCreator { get; private set; }
        public Blob PostCreatorProfilePicBlob { get; private set; }

        public bool IsLoggedIn { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            IsLoggedIn = currentUser != null;

            var post = await _postService.GetPostById(id);

            if (post != null)
            {
                Post = post;
                
                Comments = await _commentService.GetCommentsAsync(Post.Id);
                PostCreator = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Id == Post.UserId);
                PostCreatorProfilePicBlob = await _appDbContext.Blobs.FirstOrDefaultAsync(b => b.Id == PostCreator.ProfilePictureFileId);

                return Page();
            }

            return Redirect("Error");
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            var user = await _userManager.GetUserAsync(User);

            var newComment = new Comment
            {
                Id = Guid.NewGuid().ToString(),
                Content = Input.Comment,
                IsDeleted = false,
                PostId = id,
                UserId = user.Id,
                Timestamp = DateTime.Now
            };

            await _commentService.CreateCommentAsync(newComment);

            return Redirect(Url.Page("post", new { id = id }));
        }
    }
}
