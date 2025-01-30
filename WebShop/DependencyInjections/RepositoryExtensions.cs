using ControllersServices.CategoryService;
using ControllersServices.CategoryService.Interfaces;
using ControllersServices.ProductBrowserService;
using ControllersServices.ProductBrowserService.Interfaces;
using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Services.CategoryService;
using Services.CategoryService.Interfaces;

namespace WebShop.DependencyInjections
{
    public static class RepositoryExtensions
    {
        public static IServiceCollection AddReoistoryServices(this IServiceCollection services)
        {
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IDiscountRepository, DiscountRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IPhotoUrlsSetRepository, PhotoUrlSetReposiotory>();
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<ICartItemRepository, CartItemRepository>();
            return services;
        }
    }
}
