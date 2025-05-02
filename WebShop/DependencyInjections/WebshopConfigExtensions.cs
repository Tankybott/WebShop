using Utility.DiscountQueues.Interfaces;
using Utility.Common;
using Utility.Common.Interfaces;
using Utility.Queues;
using Microsoft.AspNetCore.Identity.UI.Services;
using Services.CartServices.Interfaces;
using Services.CartServices;
using Services.WebshopConfigServices.Interfaces;
using Services.WebshopConfigServices;

namespace WebShop.DependencyInjections
{
    public static class WebshopConfigExtensions
    {
        public static IServiceCollection AddWebshopConfig(this IServiceCollection services)
        {
            services.AddScoped<IFreeShippingThresholdManager, FreeShippingThresholdManager>();
            services.AddScoped<IWebshopSettingsServices, WebshopSettingsServices>();
            return services;
        }
    }
}
