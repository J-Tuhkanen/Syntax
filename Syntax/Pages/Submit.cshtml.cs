using Syntax.Models;
using Syntax.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System;
using Syntax.Data;
using System.Threading.Tasks;

namespace Syntax.Pages
{
    [Authorize]
    public class SubmitModel : PageModel
    {
        private UserManager<UserAccount> _userManager;
        private IPostService _postService;
        public SubmitModel(
            UserManager<UserAccount> userManager,
            IPostService postService)
        {
            _userManager = userManager;
            _postService = postService;

            //_postService.CreatePostAsync(new Post() { Id = Guid.NewGuid().ToString() }).GetAwaiter().GetResult();
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Title")]
            public string Title { get; set; }

            [Required]
            public string SubmitBody { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var newPost = new Post()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = user.Id,
                IsDeleted = false,
                Title = Input.Title,
                Body = Input.SubmitBody
            };

            
            if(await _postService.CreatePostAsync(newPost) != null)
            {
                return Redirect($"Post/{newPost.Id}");
            }

            return Redirect("/Error");
        }
    }
}
