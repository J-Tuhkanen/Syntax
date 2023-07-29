﻿using Microsoft.EntityFrameworkCore;
using Syntax.Core.Data;
using Syntax.Core.Models;
using Syntax.Core.Repositories.Base;

namespace Syntax.Core.Repositories
{
    public class PostRepository : RepositoryBase, IPostRepository
    {
        public PostRepository(ApplicationDbContext applicationDbContext)
            : base(applicationDbContext)
        {
        }

        public async Task<Post> CreatePostAsync(Post post)
        {
            await applicationDbContext.Posts.AddAsync(post);

            return post;
        }

        public async Task<IEnumerable<Post>> GetPostsByUserAsync(string userId, IEnumerable<string> excludedPosts, int amount)
        {
            var validPosts = applicationDbContext.Posts.Where(p =>
                p.IsDeleted == false &&
                p.UserId == userId &&
                excludedPosts.Contains(p.Id) == false).Take(amount);

            return await validPosts.ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetPostsAsync(IEnumerable<string> excludedPosts, int amount)
        {
            var validPosts = await applicationDbContext.Posts.Where(p =>
                p.IsDeleted == false &&
                excludedPosts.Contains(p.Id) == false).Take(amount).ToListAsync();
            
            return validPosts;
        }

        public async Task<Post> GetPostById(string id) => await applicationDbContext.Posts.FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);

    }
}