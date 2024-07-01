using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Syntax.API;
using Syntax.Core.Repositories;
using Syntax.Core.Services;
using System.Data.Common;
using System.Linq;

namespace Syntax.Tests.IntegrationTests
{
    internal class TestApplicationFactoryStartup : WebApplicationFactory<Program>
    {
        public static UserAccount User { get; private set; }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var dbContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                services.Remove(dbContextDescriptor);

                var dbConnectionDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbConnection));
                services.Remove(dbConnectionDescriptor);

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseNpgsql("User ID=postgres; Password=postgres; Host=localhost; Port=5432; Database=Syntax-Tests");
                });

                using (var serviceProvider = services.BuildServiceProvider())
                {
                    var userService = serviceProvider.GetRequiredService<IUserService>();
                    User = CreateUserInstance();
                    
                    var result = userService.CreateUser(User, "TimoTest", "Testi123", "timo.testi@gmail.com").GetAwaiter().GetResult();
                }
            });

            builder.UseEnvironment("Development");
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
