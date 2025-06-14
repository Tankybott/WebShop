using DataAccess.Repository.IRepository;
using Microsoft.Extensions.DependencyInjection;
using Utility.Constants;

namespace BackgroundServices.BackgroundProcessors
{
    public class OrderDeletionBackgroundService : ProcessBackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public OrderDeletionBackgroundService(IServiceScopeFactory scopeFactory) : base(1)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ProcessAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var ordersToDelete = await unitOfWork.OrderHeader
                .GetAllAsync(c => c.OrderStatus == OrderStatuses.CreatedStatus &&
                      c.CreationDate <= DateTime.UtcNow.AddMinutes(-60));

            if (ordersToDelete?.Any() == true)
            {
                unitOfWork.OrderHeader.RemoveRange(ordersToDelete);
                await unitOfWork.SaveAsync();
            }
        }
    }
}
