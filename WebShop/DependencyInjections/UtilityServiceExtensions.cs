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
            services.AddTransient<IBaseUrlRetriever, BaseUrlRetriever>();

            services.AddScoped<IImageProcessor, ImageProcessor>();
            services.AddScoped<IUserRetriver, UserRetriver>();

            services.AddSingleton<IDiscountActivationQueue, DiscountActivationQueue>();
            services.AddSingleton<IDiscountDeletionQueue, DiscountDeletionQueue>();

            return services;
        }
    }
}
