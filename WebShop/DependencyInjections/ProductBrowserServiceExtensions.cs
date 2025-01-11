using ControllersServices.CategoryService;
using ControllersServices.CategoryService.Interfaces;
using ControllersServices.ProductBrowserService;
using ControllersServices.ProductBrowserService.Interfaces;
using Services.CategoryService;
using Services.CategoryService.Interfaces;

namespace WebShop.DependencyInjections
{
    public static class ProductBrowserServiceExtensions
    {
        public static IServiceCollection AddProductBrowserServices(this IServiceCollection services)
        {
            services.AddScoped<IProductBrowserVmCreator, ProductBrowserVmCreator>();
            services.AddScoped<IProductBrowserService, ProductBrowserService>();
            return services;
        }
    }
}
