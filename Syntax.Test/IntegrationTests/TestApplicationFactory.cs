using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Syntax.API;
using Syntax.Core.Hubs;
using System.Data.Common;

namespace Syntax.Tests.IntegrationTests
{
    internal class TestApplicationFactory : WebApplicationFactory<Program>
    {
        public static UserAccount TimoTestUser { get; private set; }
        public static UserAccount ToniTestUser { get; private set; }

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

                services.Configure<IEndpointRouteBuilder>(options =>
                {
                    options.MapHub<NotificationHub>("/notification/{topicId}");
                });

                using (var serviceProvider = services.BuildServiceProvider())
                {
                    var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
                    dbContext.Database.EnsureDeletedAsync().GetAwaiter().GetResult();
                    dbContext.Database.MigrateAsync().GetAwaiter().GetResult();

                    var userService = serviceProvider.GetRequiredService<IUserService>();
                    TimoTestUser = new UserAccount();
                    ToniTestUser = new UserAccount();

                    userService.CreateUser(TimoTestUser, "TimoTest", "Testi123", "timo.testi@gmail.com").GetAwaiter().GetResult();
                    userService.CreateUser(ToniTestUser, "ToniTest", "Testi123", "toni.testi@gmail.com").GetAwaiter().GetResult();
                }
            });
            
            builder.UseEnvironment("Development");
        }
    }
}
