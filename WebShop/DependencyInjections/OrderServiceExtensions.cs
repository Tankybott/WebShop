using ControllersServices.CategoryService;
using ControllersServices.CategoryService.Interfaces;
using Services.CategoryService.Interfaces;
using Services.CategoryService;
using Services.OrderServices.Interfaces;
using Services.OrderServices;

namespace WebShop.DependencyInjections
{
    public static class OrderServiceExtensions
    {
        public static IServiceCollection AddOrderServices(this IServiceCollection services)
        {
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IOrderHeaderManager, OrderHeaderManager>();
            services.AddScoped<IOrderDetailsPriceCalculator, OrderDetailsPriceCalculator>();
            services.AddScoped<IOrderDetailsCreator, OrderDetailsCreator>();
            services.AddScoped<IOrderVMManager, OrderVMManager>();
            services.AddScoped<IStripePaymentManager, StripePaymentManager>();
            services.AddScoped<IOrderCreator, OrderCreator>();
            services.AddScoped<IOrderStockReducer, OrderStockReducer>();

            return services;
        }
    }
}
