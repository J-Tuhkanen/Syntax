using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Syntax.Models;
using Syntax.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Syntax.Pages
{
    //[Authorize]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IPostService _postService;

        public IndexModel(ILogger<IndexModel> logger, IPostService postService)
        {
            _logger = logger;
            _postService = postService;
        }

        public IEnumerable<Post> Posts { get; private set; }

        public async Task OnGetAsync()
        {
            Posts = await _postService.GetPosts();
        }
    }
}
