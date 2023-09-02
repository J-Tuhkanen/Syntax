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
using System.Text.Encodings.Web;

namespace Syntax.Pages
{
    [Authorize]
    public class SubmitModel : PageModel
    {
        private UserManager<UserAccount> _userManager;
        private IPostService _postService;
        public SubmitModel(UserManager<UserAccount> userManager, IPostService postService)
        {
            _userManager = userManager;
            _postService = postService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            private string _title;
            private string _submitBody;

            [Required]
            [Display(Name = "Title")]
            [MaxLength(80)]
            public string Title 
            {
                get => _title;
                set => _title = HtmlEncoder.Default.Encode(value);
            }

            [Required]
            public string SubmitBody 
            {
                get => _submitBody;
                set => _submitBody = HtmlEncoder.Default.Encode(value);
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                try
                {
                    var newPost = await _postService.CreatePostAsync(Input.Title, Input.SubmitBody, user.Id);
                    return Redirect(Url.Page("post", new { id = newPost.Id }));
                }
                catch
                {
                    return Redirect("/Error");
                }
            }
            // TODO: Display invalid field error messages
            return Page();
        }
    }
}
