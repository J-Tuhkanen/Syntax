using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Syntax.Models;
using Syntax.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Syntax.Pages
{
    [Authorize]
    public class PostModel : PageModel
    {
        private IPostService _postService;
        private ICommentService _commentService;
        private UserManager<UserAccount> _userManager;

        public PostModel(IPostService postService, ICommentService commentService, UserManager<UserAccount> userManager)
        {
            _postService = postService;
            _commentService = commentService;
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public string Comment { get; set; }
        }

        public Post Post { get; private set; }
        public IEnumerable<Comment> Comments { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var post = await _postService.GetPostById(id);

            if(post != null)
            {
                Post = post;
                Comments = await _commentService.GetCommentsAsync(Post.Id);

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
                UserId = user.Id
            };

            await _commentService.CreateCommentAsync(newComment);

            return Redirect($"/post/{id}");
        }
    }
}
