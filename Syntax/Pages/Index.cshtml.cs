using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Syntax.Services;
using Syntax.Wrappers;
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

        public IEnumerable<PostWrapper> Posts { get; private set; }

        public async Task OnGetAsync()
        {
            Posts = (await _postService.GetPosts()).Select(p => new PostWrapper(p));
        }
    }
}
