using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using AngularBBS.Data;
using AngularBBS.Models;
using AngularBBS.Services;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace AngularBBS
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see https://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
            GithubConfig.ClientId = Configuration["GitHub:ClientId"];
            GithubConfig.Secret = Configuration["GitHub:ClientSecret"];
        }

        public IConfigurationRoot Configuration { get; }

        private OAuthOptions GitHubOptions =>
            new OAuthOptions
            {
                AuthenticationScheme = "GitHub",
                DisplayName = "GitHub",
                ClientId = GithubConfig.ClientId,
                ClientSecret = GithubConfig.Secret,
                CallbackPath = new PathString("/login-github"),
                AuthorizationEndpoint = GithubConfig.AuthorizeEndpoint,
                TokenEndpoint = GithubConfig.TokenEndpoint,
                UserInformationEndpoint = GithubConfig.UserInfoEndPoint,
                ClaimsIssuer = GithubConfig.ClaimIssure,
                SaveTokens = true,
                Scope = {"public_repo", "user:email"},
                // Retrieving user information is unique to each provider.
                Events = new OAuthEvents
                {
                    OnCreatingTicket = async context => { await CreateGitHubAuthTicket(context); }
                }
            };

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthorization();
            services.AddMvc();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.AddTransient<HttpClient, HttpClient>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
          
            app.UseStaticFiles();

             app.UseIdentity();

            // Add external authentication middleware below. To configure them please see https://go.microsoft.com/fwlink/?LinkID=532715
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = "cookies",
                AutomaticChallenge = true
            });

          
            app.UseOAuthAuthentication(GitHubOptions);
            
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private static async Task CreateGitHubAuthTicket(OAuthCreatingTicketContext context)
        {
            // Get the GitHub user
            var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await context.Backchannel.SendAsync(request, context.HttpContext.RequestAborted);
            response.EnsureSuccessStatusCode();

            var user = JObject.Parse(await response.Content.ReadAsStringAsync());

            AddClaims(context, user);
        }

        private static void AddClaims(OAuthCreatingTicketContext context, JObject user)
        {
            var identifier = user.Value<string>("id");
            if (!string.IsNullOrEmpty(identifier))
            {
                context.Identity.AddClaim(new Claim(
                    ClaimTypes.NameIdentifier, identifier,
                    ClaimValueTypes.String, context.Options.ClaimsIssuer));
            }

            var userName = user.Value<string>("login");
            if (!string.IsNullOrEmpty(userName))
            {
                context.Identity.AddClaim(new Claim(
                    ClaimsIdentity.DefaultNameClaimType, userName,
                    ClaimValueTypes.String, context.Options.ClaimsIssuer));
            }

            var name = user.Value<string>("name");
            if (!string.IsNullOrEmpty(name))
            {
                context.Identity.AddClaim(new Claim(
                    GithubConfig.NameClaimType, name,
                    ClaimValueTypes.String, context.Options.ClaimsIssuer));
            }

            var link = user.Value<string>("url");
            if (!string.IsNullOrEmpty(link))
            {
                context.Identity.AddClaim(new Claim(
                    GithubConfig.UrlClaimType, link,
                    ClaimValueTypes.String, context.Options.ClaimsIssuer));
            }

            var email = user.Value<string>("email");
            if (!string.IsNullOrEmpty(email))
            {
                context.Identity.AddClaim(new Claim(
                    GithubConfig.EmailClaimType, email,
                    ClaimValueTypes.String, context.Options.ClaimsIssuer));
            }
        }
    }
}