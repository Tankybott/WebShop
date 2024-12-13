using ControllersServices.AdminCategoryService;
using ControllersServices.AdminCategoryService.Interfaces;

namespace WebShop.DependencyInjections
{
    public static class AdminCategoryServiceExtensions
    {
        public static IServiceCollection AddAdminCategoryServices(this IServiceCollection services)
        {
            services.AddScoped<ICategoryHierarchyCreator, CategoryHierarchyCreator>();
            services.AddScoped<IAdminCategoryVMCreator, AdminCategoryVMCreator>();
            services.AddScoped<ICategoryHierarchyRetriver, CategoryHierarchyRetriver>();
            services.AddScoped<IAdminCategoryService, AdminCategoryService>();


            return services;
        }
    }
}
