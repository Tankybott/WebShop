using DataAccess.Repository.IRepository;
using Microsoft.Extensions.DependencyInjection;

namespace BackgroundServices.BackgroundProcessors
{
    public class CartDeletionBackgroundService : ProcessBackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public CartDeletionBackgroundService(IServiceScopeFactory scopeFactory) : base(1)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ProcessAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var cartsToDelete = await unitOfWork.Cart.GetAllAsync(c => c.ExpiresTo != null && c.ExpiresTo <= DateTime.UtcNow);
            if (cartsToDelete?.Any() == true)
            {
                unitOfWork.Cart.RemoveRange(cartsToDelete);
                await unitOfWork.SaveAsync();
            }
        }
    }
}
