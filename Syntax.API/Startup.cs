﻿using Microsoft.EntityFrameworkCore;
using Syntax.Core.Data;
using Syntax.Core.Models;
using Syntax.Core.Repositories;
using Syntax.Core.Services.Base;
using Syntax.Core.Services;

namespace Syntax.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            // Database configuration
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            // Configure identity and required settings for each user
            services.AddDefaultIdentity<UserAccount>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 0;
            }).AddEntityFrameworkStores<ApplicationDbContext>();

            // Add services to dependency injection to be injectable whenever needed
            services.AddTransient<IPostService, PostService>();
            services.AddTransient<ICommentService, CommentService>();
            services.AddTransient<IFileService, FileService>();
            services.AddTransient<IUserService, UserService>();

            services.AddTransient<UnitOfWork>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts
                app.UseHsts();
            }

            CreateFilesFolderIfDoesntExist(env);

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void CreateFilesFolderIfDoesntExist(IWebHostEnvironment env)
        {
            var userFilesPath = Path.Combine(env.WebRootPath, "files");

            if (Directory.Exists(userFilesPath) == false)
                Directory.CreateDirectory(userFilesPath);
        }
    }
}