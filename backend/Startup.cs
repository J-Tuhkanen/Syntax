using Microsoft.EntityFrameworkCore;
using Syntax.Core.Data;
using Syntax.Core.Models;
using Syntax.Core.Services.Base;
using Syntax.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Syntax.Core.Hubs;
using Azure.Core;
using Azure.Identity;
using Azure.Storage.Blobs;

namespace Syntax.API
{
    public class Startup
    {
        private readonly string allowSpecificOrigin = "AllowSpecificOrigin";

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            HostEnvironment = environment;
        }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment HostEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            // Database configuration. Nothing secret lives in appsettings.json:
            //   Azure: ConnectionStrings__DefaultConnection (Key Vault reference)
            //   local: dotnet user-secrets or the same environment variable
            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString) && !HostEnvironment.IsDevelopment())
                throw new InvalidOperationException("ConnectionStrings:DefaultConnection is not configured.");

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));
            services.AddSignalR();
            services.AddCors(options =>
            {
                options.AddPolicy(name: allowSpecificOrigin, policy => {
                    policy.WithOrigins("https://localhost:3000");
                    policy.AllowAnyHeader();
                    policy.AllowAnyMethod();
                    policy.AllowCredentials();
                });
            });

            services.Configure<JsonOptions>(o =>
            {
                o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
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
            })
                .AddSignInManager<SignInManager<UserAccount>>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
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

            // Add services to dependency injection to be injectable whenever needed
            services.AddTransient<ITopicService, TopicService>();
            services.AddTransient<ICommentService, CommentService>();
            AddFileStorage(services);
            services.AddTransient<IUserService, UserService>();
            
            services.AddWebEncoders();
            services.AddDistributedMemoryCache();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //CreateRoles(serviceProvider).GetAwaiter().GetResult();
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts
                app.UseHsts();
            }

            app.UseRouting();
            app.UseCors(allowSpecificOrigin);
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<NotificationHub>("/notification/{topicId}");
                endpoints.MapControllers();
            });
            app.UseResponseCaching();  
        }

        /// <summary>
        /// Profile pictures go to Azure Blob storage when Storage:BlobServiceUri is
        /// set (Azure), and to wwwroot/users on local disk otherwise (development).
        /// Either way they are served by UserFilesController at /users/{userId}/{file}.
        /// </summary>
        private void AddFileStorage(IServiceCollection services)
        {
            var blobServiceUri = Configuration["Storage:BlobServiceUri"];

            if (string.IsNullOrWhiteSpace(blobServiceUri))
            {
                services.AddTransient<IFileService>(provider => new LocalFileService(
                    Path.Combine(provider.GetRequiredService<IWebHostEnvironment>().ContentRootPath, "wwwroot", "users")));
                return;
            }

            var containerName = Configuration["Storage:AvatarContainer"] ?? "avatars";

            services.AddSingleton(new BlobServiceClient(new Uri(blobServiceUri), AzureCredential)
                .GetBlobContainerClient(containerName));
            services.AddTransient<IFileService, BlobFileService>();
        }

        /// <summary>
        /// Managed identity in Azure (AZURE_CLIENT_ID selects the user-assigned
        /// identity); developer sign-ins such as `az login` locally.
        /// </summary>
        private TokenCredential AzureCredential => _azureCredential ??= new DefaultAzureCredential(
            new DefaultAzureCredentialOptions { ManagedIdentityClientId = Configuration["AZURE_CLIENT_ID"] });

        private TokenCredential? _azureCredential;

        private async Task CreateRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            foreach (var roleName in ApplicationConstants.UserRoles.GetRoles())
            {
                if (await roleManager.RoleExistsAsync(roleName) == false)
                    await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }
}
