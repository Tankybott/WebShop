using ControllersServices.CategoryService;
using ControllersServices.ProductManagement;
using Services.DiscountService;
using Services.DiscountService.Interfaces;
using Services.PhotoService;
using Services.PhotoService.Interfaces;
using Services.PhotoSetService;
using Services.PhotoSetService.Interfaces;
using Services.ProductManagement;
using Services.ProductManagement.Interfaces;
using Services.ProductService;
using Services.ProductService.Interfaces;

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
            services.AddScoped<IProductDiscountUpserter, Services.ProductService.ProductDiscountUpserter>();
            services.AddScoped<IProductRemover, ProductRemover>();
            services.AddScoped<IProductTableDtoRetriver, ProductTableDtoRetriver>();
            services.AddScoped<IMainPhotoSetSynchronizer, MainPhotoSetSynchronizer>();
            services.AddScoped<IProductPriceRetriver, ProductPriceRetriver>();


            return services;
        }
    }
}