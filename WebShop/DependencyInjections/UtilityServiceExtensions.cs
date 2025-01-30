using Utility.DiscountQueues.Interfaces;
using Utility.Common;
using Utility.Common.Interfaces;
using Utility.Queues;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace WebShop.DependencyInjections
{
    public static class UtilityServiceExtensions
    {
        public static IServiceCollection AddUtilityServices(this IServiceCollection services)
        {
            services.AddTransient<IPathCreator, PathCreator>();
            services.AddTransient<IFileService, FileService>();
            services.AddTransient<IFileNameCreator, FileNameCreator>();

            services.AddScoped<IImageProcessor, ImageProcessor>();
            services.AddScoped<IEmailSender, EmailSender>();

            services.AddSingleton<IActivationDiscountQueue, ActivationDiscountQueue>();
            services.AddSingleton<IDeletionDiscountQueue, DeletionDiscountQueue>();

            return services;
        }
    }
}
