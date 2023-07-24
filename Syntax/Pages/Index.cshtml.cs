using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Syntax.Core.Models;
using Syntax.Core.Services;
using Syntax.Core.Services.Interfaces;
using Syntax.Core.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Syntax.Pages
{
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
            IEnumerable<Post> postQueryResult = await _postService.GetPostsAsync();

            IEnumerable<PostWrapper> postWrapper = postQueryResult.Select(p => new PostWrapper(p));

            Posts = postWrapper;
        }
    }
}
