using ControllersServices.CategoryService;
using ControllersServices.CategoryService.Interfaces;
using ControllersServices.ProductManagement;
using ControllersServices.ProductManagement.Interfaces;
using Services.ProductService;
using Services.ProductService.Interfaces;

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
            services.AddScoped<IProductTableDtoRetriver, ProductTableDtoRetriver>();

            return services;
        }
    }
}