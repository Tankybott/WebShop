using ControllersServices.AdminCategoryService;
using ControllersServices.AdminCategoryService.Interfaces;
using ControllersServices.ProductService;
using ControllersServices.ProductService.Interfaces;

namespace WebShop.DependencyInjections
{
    public static class ProductServiceExtensions
    {
        public static IServiceCollection AddProductServices(this IServiceCollection services)
        {
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IProductVMCreator, ProductVMCreator>();
            services.AddScoped<IProductPhotoService, ProductPhotoServices>();
            services.AddScoped<IProductUpserter, ProductUpserter>();
            services.AddScoped<IDiscountService, DiscountService>();

            return services;
        }
    }
}