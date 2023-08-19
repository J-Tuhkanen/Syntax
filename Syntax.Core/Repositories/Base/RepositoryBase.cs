using Syntax.Core.Data;

namespace Syntax.Core.Repositories.Base
{
    public abstract class RepositoryBase : IDisposable
    {
        private bool isDisposed = false;
        protected readonly ApplicationDbContext applicationDbContext;

        public RepositoryBase(ApplicationDbContext applicationDbContext) 
        {
            this.applicationDbContext = applicationDbContext;
        }

        public async Task SaveChangesAsync() 
            => await applicationDbContext.SaveChangesAsync();

        public async void Dispose()
        {
            if (isDisposed)
            {
                await applicationDbContext.DisposeAsync();
                isDisposed = true;
            }

            GC.SuppressFinalize(this);
        }
    }
}
