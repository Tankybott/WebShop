using Utility.DiscountQueues.Interfaces;
using Utility.Common;
using Utility.Common.Interfaces;
using Utility.Queues;
using Microsoft.AspNetCore.Identity.UI.Services;
using Services.CartServices.Interfaces;
using Services.CartServices;
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
