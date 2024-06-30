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
    internal class TestApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseNpgsql("User ID=postgres; Password=postgres; Host=localhost; Port=5432; Database=Syntax-Tests");
                });
                //services.AddTransient<ITopicService, TopicService>();
                //services.AddTransient<ICommentService, CommentService>();
                //services.AddTransient<IFileService, FileService>();
                //services.AddTransient<IUserService, UserService>();
                //services.AddTransient<UnitOfWork>();

                //services.AddIdentityCore<UserAccount>(options =>
                //{
                //    options.SignIn.RequireConfirmedAccount = false;
                //    options.Password.RequireDigit = false;
                //    options.Password.RequireNonAlphanumeric = false;
                //    options.Password.RequiredUniqueChars = 0;
                //    options.Password.RequireUppercase = false;
                //    options.Password.RequiredLength = 0;
                //})
                //    .AddSignInManager<SignInManager<UserAccount>>()
                //    .AddEntityFrameworkStores<ApplicationDbContext>();

                //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
                //{
                //    options.Cookie.Name = "syntax.user";
                //    options.Cookie.HttpOnly = true;
                //    options.SlidingExpiration = true;
                //    options.ExpireTimeSpan = new TimeSpan(1, 0, 0);
                //    options.Events.OnRedirectToLogin = (context) =>
                //    {
                //        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                //        return Task.CompletedTask;
                //    };
                //});

                using (var serviceProvider = services.BuildServiceProvider())
                {
                    var userService = serviceProvider.GetRequiredService<IUserService>();

                    var user = CreateUserInstance();

                    var result = userService.CreateUser(user, "TimoTest", "Testi123", "timo.testi@gmail.com").GetAwaiter().GetResult();
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
