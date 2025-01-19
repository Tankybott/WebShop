using ControllersServices.CategoryService;
using ControllersServices.ProductManagement;
using Services.PhotoService;
using Services.PhotoService.Interfaces;
using Services.PhotoService.Interfaces.DiscountService;
using Services.PhotoService.Interfaces.DiscountService.Interfaces;
using Services.PhotoSetService;
using Services.PhotoSetService.Interfaces;
using Services.ProductManagement;
using Services.ProductManagement.Interfaces;
using Services.ProductService;

namespace WebShop.DependencyInjections
{
    public static class ProductServiceExtensions
    {
        public static IServiceCollection AddProductServices(this IServiceCollection services)
        {
            services.AddScoped<IProductService, ProductServices>();
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