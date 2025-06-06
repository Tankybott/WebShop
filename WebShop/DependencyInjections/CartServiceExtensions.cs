
using Services.CarrierService.Interfaces;
using Services.CarrierService;

namespace WebShop.DependencyInjections
{
    public static class CartServiceExtensions
    {
        public static IServiceCollection AddCarrierService(this IServiceCollection services)
        {
            services.AddScoped<ICarrierService, CarrierService>();
            return services;
        }
    }
}
