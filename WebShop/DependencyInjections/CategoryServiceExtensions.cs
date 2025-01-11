using ControllersServices.CategoryService;
using ControllersServices.CategoryService.Interfaces;
using Services.CategoryService.Interfaces;
using Services.CategoryService;

namespace WebShop.DependencyInjections
{
    public static class AdminCategoryServiceExtensions
    {
        public static IServiceCollection AddCategoryServices(this IServiceCollection services)
        {
            services.AddScoped<ICategoryHierarchyCreator, CategoryHierarchyCreator>();
            services.AddScoped<ICategoryVMCreator, CategoryVMCreator>();
            services.AddScoped<ICategoryHierarchyRetriver, CategoryHierarchyRetriver>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICategoryReletedProductRemover, CategoryReletedProductRemover>();
            services.AddTransient<ICategoryIdRetriver, CategoryIdRetriver>();


            return services;
        }
    }
}
