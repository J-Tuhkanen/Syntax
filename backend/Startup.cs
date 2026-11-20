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
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using System.Threading.RateLimiting;

namespace Syntax.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            HostEnvironment = environment;
        }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment HostEnvironment { get; }

        private bool RateLimitingEnabled => Configuration.GetValue("RateLimiting:Enabled", true);

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

            // No CORS: in production nginx serves the SPA and proxies the API on the
            // same origin, and in development the Vite dev server does the same.

            // The API is only reachable inside the Container Apps environment, through
            // the frontend's nginx, which sends exactly one client address in
            // X-Forwarded-For. The proxies' own addresses aren't fixed, so trust them all.
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
                options.ForwardLimit = null;
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

            // Secure cookies everywhere except Development, where the Vite dev server
            // and the integration tests use plain http://localhost.
            var cookieSecurePolicy = HostEnvironment.IsDevelopment()
                ? CookieSecurePolicy.SameAsRequest
                : CookieSecurePolicy.Always;

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.SecurePolicy = cookieSecurePolicy;
                options.Cookie.SameSite = SameSiteMode.Lax;
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = "syntax.user";
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = cookieSecurePolicy;
                    options.Cookie.SameSite = SameSiteMode.Lax;
                    options.SlidingExpiration = true;
                    options.ExpireTimeSpan = new TimeSpan(1, 0, 0);
                    options.Events.OnRedirectToLogin = (context) =>
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return Task.CompletedTask;
                    };
                });

            AddDataProtection(services);

            if (RateLimitingEnabled)
                AddRateLimiting(services);

            // Add services to dependency injection to be injectable whenever needed
            services.AddTransient<ITopicService, TopicService>();
            services.AddTransient<ICommentService, CommentService>();
            AddFileStorage(services);
            services.AddTransient<IUserService, UserService>();

            services.AddWebEncoders();
            services.AddDistributedMemoryCache();
            services.AddProblemDetails();
            services.AddHealthChecks();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //CreateRoles(serviceProvider).GetAwaiter().GetResult();

            // First, so everything after it sees the client's address and scheme.
            app.UseForwardedHeaders();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                // ProblemDetails response instead of re-executing a (non-existent) /Error page.
                app.UseExceptionHandler();
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts
                app.UseHsts();
            }

            app.UseRouting();

            if (RateLimitingEnabled)
                app.UseRateLimiter();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/healthz").DisableRateLimiting();
                endpoints.MapHub<NotificationHub>("/notification/{topicId}");
                endpoints.MapControllers();
            });
            app.UseResponseCaching();
        }

        /// <summary>
        /// The API scales to zero and may move between replicas, so the Data
        /// Protection key ring (it encrypts the auth cookies) lives in Blob storage,
        /// wrapped with a Key Vault key. Without that configuration (development)
        /// the default local key ring is used.
        /// </summary>
        private void AddDataProtection(IServiceCollection services)
        {
            var dataProtection = services.AddDataProtection().SetApplicationName("Syntax");

            var keyRingBlobUri = Configuration["DataProtection:BlobUri"];
            var keyVaultKeyId = Configuration["DataProtection:KeyVaultKeyId"];

            if (!string.IsNullOrWhiteSpace(keyRingBlobUri) && !string.IsNullOrWhiteSpace(keyVaultKeyId))
            {
                dataProtection
                    .PersistKeysToAzureBlobStorage(new Uri(keyRingBlobUri), AzureCredential)
                    .ProtectKeysWithAzureKeyVault(new Uri(keyVaultKeyId), AzureCredential);
            }
            else if (!HostEnvironment.IsDevelopment())
            {
                // Keys kept inside the container would be lost on every restart,
                // signing everybody out.
                throw new InvalidOperationException("DataProtection:BlobUri and DataProtection:KeyVaultKeyId must be configured outside Development.");
            }
        }

        /// <summary>
        /// Built-in rate limiter (there is no API gateway): a per-client budget for
        /// every request, and a much smaller one for sign-in and registration.
        /// Limits come from the RateLimiting section; disabled in Development.
        /// </summary>
        private void AddRateLimiting(IServiceCollection services)
        {
            var permitLimit = Configuration.GetValue("RateLimiting:PermitLimit", 300);
            var authenticationPermitLimit = Configuration.GetValue("RateLimiting:AuthenticationPermitLimit", 10);

            static string ClientKey(HttpContext context) =>
                context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            static FixedWindowRateLimiterOptions PerMinute(int permits) => new()
            {
                PermitLimit = permits,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
            };

            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                    RateLimitPartition.GetFixedWindowLimiter(ClientKey(context), _ => PerMinute(permitLimit)));

                options.AddPolicy(ApplicationConstants.RateLimitPolicies.Authentication, context =>
                    RateLimitPartition.GetFixedWindowLimiter(ClientKey(context), _ => PerMinute(authenticationPermitLimit)));
            });
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
