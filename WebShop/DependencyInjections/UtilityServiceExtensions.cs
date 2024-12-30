using ControllersServices.AdminCategoryService.Interfaces;
using ControllersServices.AdminCategoryService;
using ControllersServices.Utilities.Interfaces;
using ControllersServices.Utilities;
using Utility.DiscountQueues.Interfaces;

namespace WebShop.DependencyInjections
{
    public static class UtilityServiceExtensions
    {
        public static IServiceCollection AddUtilityServices(this IServiceCollection services)
        {
            services.AddScoped<IFileNameCreator, FileNameCreator>();
            services.AddScoped<IPathCreator, PathCreator>();
            services.AddScoped<IFileService, FileService>();

            services.AddSingleton<IActivationDiscountQueue, ActivationDiscountQueue>();
            services.AddSingleton<IDeletionDiscountQueue, DeletionDiscountQueue>();

            return services;
        }
    }
}
