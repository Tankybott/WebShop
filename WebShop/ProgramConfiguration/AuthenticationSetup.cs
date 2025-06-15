using DataAccess;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Services.CartServices.Interfaces;
using System.Security.Claims;
using Models;
using System.Net.Mime;

namespace WebShop.ProgramConfiguration
{
    public static class AuthenticationSetup
    {
        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>()
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
                options.SignIn.RequireConfirmedEmail = true;
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login";
                options.LogoutPath = "/Identity/Account/Logout";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";

                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.SlidingExpiration = true;
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.IsEssential = true;

                options.Events = new CookieAuthenticationEvents
                {
                    OnSigningOut = async context =>
                    {
                        var unitOfWork = context.HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
                        var userId = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                        if (!string.IsNullOrEmpty(userId))
                        {
                            var cart = await unitOfWork.Cart.GetAsync(c => c.UserId == userId);
                            if (cart != null)
                            {
                                cart.ExpiresTo = DateTime.UtcNow.AddMinutes(2);
                                unitOfWork.Cart.Update(cart);
                                await unitOfWork.SaveAsync();
                            }
                        }
                    },

                    OnValidatePrincipal = async context =>
                    {
                        var unitOfWork = context.HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
                        if (context.Principal.Identity?.IsAuthenticated == true)
                        {
                            var userId = context.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
                            var cart = await unitOfWork.Cart.GetAsync(c => c.UserId == userId);
                            if (cart != null)
                            {
                                cart.ExpiresTo = null;
                                unitOfWork.Cart.Update(cart);
                                await unitOfWork.SaveAsync();
                            }
                        }
                    },

                    OnRedirectToAccessDenied = context =>
                    {
                        if (IsApiRequest(context.Request))
                        {
                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            context.Response.ContentType = MediaTypeNames.Application.Json;
                            return context.Response.WriteAsync("{\"error\":\"Access denied. You are not authorized to perform this action.\"}");
                        }

                        context.Response.Redirect(context.Options.AccessDeniedPath);
                        return Task.CompletedTask;
                    }
                };
            });

            return services;
        }

        private static bool IsApiRequest(HttpRequest request)
        {
            return request.Path.StartsWithSegments("/api") ||
                   request.Headers["Accept"].Any(h => h.Contains("application/json", StringComparison.OrdinalIgnoreCase));
        }
    }
}
