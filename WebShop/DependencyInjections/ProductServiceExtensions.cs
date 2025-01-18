using ControllersServices.CategoryService;
using ControllersServices.ProductManagement;
using ControllersServices.ProductManagement.Interfaces;
using Services.ProductService;
using Services.ProductService.DiscountRelated;
using Services.ProductService.Interfaces;
using Services.ProductService.PhotoRelated;

namespace WebShop.DependencyInjections
{
    public static class ProductServiceExtensions
    {
        public static IServiceCollection AddProductServices(this IServiceCollection services)
        {
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IProductVMCreator, ProductVMCreator>();
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddScoped<IProductPhotoUpserter, ProductPhotoUpserter>();
            services.AddScoped<IPhotoSetService, PhotoSetService>();
            services.AddScoped<IProductUpserter, ProductUpserter>();
            services.AddScoped<IDiscountService, DiscountService>();
            services.AddScoped<IProductDiscountUpserter, ProductDiscountUpserter>();
            services.AddScoped<IProductRemover, ProductRemover>();
            services.AddScoped<IProductTableDtoRetriver, ProductTableDtoRetriver>();
            services.AddScoped<IMainPhotoSetSynchronizer, MainPhotoSetSynchronizer>();



            return services;
        }
    }
}