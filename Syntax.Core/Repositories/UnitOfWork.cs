using Syntax.Core.Data;
using Syntax.Core.Repositories.Base;

namespace Syntax.Core.Repositories
{
    public class UnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;

        public ITopicRepository Topics { get; }
        public ICommentRepository Comments { get; }
        public IUserRepository Users { get; }

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;

            Topics = new TopicRepository(_dbContext);
            Comments = new CommentRepository(_dbContext);
            Users = new UserRepository(_dbContext);
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
