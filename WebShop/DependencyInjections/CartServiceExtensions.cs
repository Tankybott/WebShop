using Utility.DiscountQueues.Interfaces;
using Utility.Common;
using Utility.Common.Interfaces;
using Utility.Queues;
using Microsoft.AspNetCore.Identity.UI.Services;
using Services.CartServices.Interfaces;
using Services.CartServices;

namespace WebShop.DependencyInjections
{
    public static class CartServiceExtensions
    {
        public static IServiceCollection AddCartServices(this IServiceCollection services)
        {
            services.AddScoped<ICartItemCreator, CartItemCreator>();
            services.AddScoped<ICartRetriver, CartRetriver>();
            services.AddScoped<ICartServices, CartServices>();
            services.AddScoped<ICartPriceSynchronizer, CartPriceSynchronizer>();
            services.AddScoped<ICartItemsQuantityRetriver, CartItemsQuantityRetriver>();
            services.AddScoped<ICartItemRemover, CartItemRemover>();
            services.AddScoped<ICartItemAdder, CartItemAdder>();
            services.AddScoped<ICartItemQuantityUpdater, CartItemQuantityUpdater>();
            services.AddScoped<ICartItemQuantityValidator, CartItemQuantityValidator>();
            services.AddScoped<ICartDeletionQueueManager, CartDeletionQueueManager>();
            return services;
        }
    }
}
