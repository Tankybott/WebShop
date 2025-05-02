using Utility.DiscountQueues.Interfaces;
using Utility.Common;
using Utility.Common.Interfaces;
using Utility.Queues;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.SignalR;
using Utility.Queues.Interfaces;

namespace WebShop.DependencyInjections
{
    public static class UtilityServiceExtensions
    {
        public static IServiceCollection AddUtilityServices(this IServiceCollection services)
        {
            services.AddTransient<IPathCreator, PathCreator>();
            services.AddTransient<IFileService, FileService>();
            services.AddTransient<IFileNameCreator, FileNameCreator>();
            services.AddTransient<IBaseUrlRetriever, BaseUrlRetriever>();

            services.AddScoped<IImageProcessor, ImageProcessor>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<IUserRetriver, UserRetriver>();

            services.AddSingleton<IDiscountActivationQueue, DiscountActivationQueue>();
            services.AddSingleton<IDiscountDeletionQueue, DiscountDeletionQueue>();
            services.AddSingleton<ICartDeletionQueue, CartDeletionQueue>();

            return services;
        }
    }
}
