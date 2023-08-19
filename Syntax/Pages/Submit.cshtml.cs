using Syntax.Core.Models;
using Syntax.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System;
using Syntax.Core.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Syntax.Core.Services.Base;

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
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Title")]
            [MaxLength(80)]
            public string Title { get; set; }

            [Required]
            public string SubmitBody { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var newPost = new Post()
                {
                    UserId = user.Id,
                    IsDeleted = false,
                    Title = Input.Title,
                    Body = Input.SubmitBody,
                    Timestamp = DateTime.Now
                };
                        
                if(await _postService.CreatePostAsync(newPost) != null)
                {
                    return Redirect(Url.Page("post", new { id = newPost.Id }));
                }

                return Redirect("/Error");
            }

            return Page();
        }
    }
}
