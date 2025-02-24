using DataAccess;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Services.CartServices.Interfaces;
using System.Security.Claims;
using Models; 

namespace WebShop.ProgramConfiguration
{
    public static class AuthenticationSetup
    {
        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
                options.User.RequireUniqueEmail = true;
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login";
                options.LogoutPath = "/Identity/Account/Logout";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";

                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.SlidingExpiration = true;
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.IsEssential = true;

                options.Events = new CookieAuthenticationEvents
                {
                    OnSigningOut = async context =>
                    {
                        var cartQueueManager = context.HttpContext.RequestServices.GetRequiredService<ICartDeletionQueueManager>();

                        var userId = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                        if (!string.IsNullOrEmpty(userId))
                        {
                            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
                            {
                                new Claim(ClaimTypes.NameIdentifier, userId)
                            }, context.Options.LoginPath));

                            await cartQueueManager.EnqueueUsersCart(user);
                        }
                    },

                    OnValidatePrincipal = async context =>
                    {
                        var cartQueueManager = context.HttpContext.RequestServices.GetRequiredService<ICartDeletionQueueManager>();

                        if (context.Principal.Identity?.IsAuthenticated == true)
                        {
                            await cartQueueManager.DequeueUsersCart(context.Principal);
                        }
                    }
                };
            });

            return services;
        }
    }
}
