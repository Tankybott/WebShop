using ControllersServices.AdminCategoryService;
using ControllersServices.AdminCategoryService.Interfaces;
using ControllersServices.ProductManagement;
using ControllersServices.ProductManagement.Interfaces;

namespace WebShop.DependencyInjections
{
    public static class ProductServiceExtensions
    {
        public static IServiceCollection AddProductServices(this IServiceCollection services)
        {
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IProductVMCreator, ProductVMCreator>();
            services.AddScoped<IProductPhotoService, ProductPhotoService>();
            services.AddScoped<IProductUpserter, ProductUpserter>();
            services.AddScoped<IDiscountService, DiscountService>();
            services.AddScoped<IProductRemover, ProductRemover>();

            return services;
        }
    }
}