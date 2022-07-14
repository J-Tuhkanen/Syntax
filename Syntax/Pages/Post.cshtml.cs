using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Syntax.Models;
using Syntax.Services;
using System.Threading.Tasks;

namespace Syntax.Pages
{
    public class PostModel : PageModel
    {
        private IPostService _postService;

        public PostModel(IPostService postService)
        {
            _postService = postService;
        }

        public Post Post { get; private set; }

        public IActionResult OnGet(string id)
        {
            var post = _postService.GetPostById(id);

            if(post != null)
            {
                Post = post;
                return Page();
            }

            return Redirect("Error");
        }
    }
}
