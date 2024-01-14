using Syntax.Core.Data;
using Syntax.Core.Repositories.Base;

namespace Syntax.Core.Repositories
{
    public class UnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;

        public IPostRepository Post { get; }
        public ICommentRepository Comment { get; }
        public IUserRepository User { get; }

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;

            Post = new PostRepository(_dbContext);
            Comment = new CommentRepository(_dbContext);
            User = new UserRepository(_dbContext);
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
