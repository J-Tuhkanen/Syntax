﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Syntax.Core.Data;
using Syntax.Core.Models;
using Syntax.Core.Repositories.Base;
using Syntax.Core.Repositories;
using Syntax.Core.Services.Base;
using Syntax.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

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
                options.UseNpgsql("User ID=postgres; Password=postgres; Host=localhost; Port=5432; Database=Syntax-Tests");
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
            })
                .AddSignInManager<SignInManager<UserAccount>>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            _services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.Cookie.Name = "syntax.user";
                options.Cookie.HttpOnly = true;
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = new TimeSpan(1, 0, 0);
                options.Events.OnRedirectToLogin = (context) =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };
            });

        }

        protected void InjectServiceDependencies()
        {
            _services.AddTransient<ITopicService, TopicService>();
            _services.AddTransient<ICommentService, CommentService>();
            _services.AddTransient<IFileService, FileService>();
            _services.AddTransient<IUserService, UserService>();
            _services.AddTransient<UnitOfWork>();
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