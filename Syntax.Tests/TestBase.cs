using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Syntax.Core.Data;
using Syntax.Core.Models;
using Syntax.Core.Repositories.Base;
using Syntax.Core.Repositories;
using Syntax.Core.Services.Base;
using Syntax.Core.Services;

namespace Syntax.Tests
{
    public abstract class TestBase
    {
        private readonly ServiceCollection _services;
        private ServiceProvider _serviceProvider;
        public TestBase()
        {
            _services = new ServiceCollection();
            
            ConfigureDatabase();
            ConfigureIdentity();
            InjectServiceDependencies();

            _serviceProvider = _services.BuildServiceProvider();            
        }

        protected T GetService<T>() 
            where T : class
        {
            return _serviceProvider.GetService<T>();   
        }

        protected void ConfigureDatabase()
        {
            _services.AddDbContext<ApplicationDbContext>(options =>
            {
                //options.UseSqlServer("Server=127.0.0.1,1433;Database=Syntax-Tests;User Id=sa;Password=SyntaxDatabase123;");
                options.UseSqlServer("Server=localhost;Database=Syntax-Tests;Trusted_Connection=True;MultipleActiveResultSets=true");
                options.UseLazyLoadingProxies();
            });
        }

        protected void ConfigureIdentity()
        {
            _services.AddIdentityCore<UserAccount>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 0;
            }).AddEntityFrameworkStores<ApplicationDbContext>();
        }

        protected void InjectServiceDependencies()
        {
            _services.AddTransient<IPostService, PostService>();
            _services.AddTransient<ICommentService, CommentService>();
            _services.AddTransient<IFileService, FileService>();
            _services.AddTransient<IUserService, UserService>();
            _services.AddTransient<IPostRepository, PostRepository>();
            _services.AddTransient<ICommentRepository, CommentRepository>();
            _services.AddTransient<IUserRepository, UserRepository>();
        }

        protected UserAccount CreateUserInstance()
        {
            try
            {
                return Activator.CreateInstance<UserAccount>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(UserAccount)}'. " +
                    $"Ensure that '{nameof(UserAccount)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }
    }
}