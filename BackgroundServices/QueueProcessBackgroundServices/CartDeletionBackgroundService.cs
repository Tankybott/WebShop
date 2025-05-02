
using DataAccess.Repository.IRepository;
using Microsoft.Extensions.DependencyInjection;
using Models;
using Utility.Queues.Interfaces;

namespace BackgroundServices.QueueProcessBackgroundServices
{
    public class CartDeletionBackgroundService : QueueProcessBackgroundService<Cart>
    {
        public CartDeletionBackgroundService(ICartDeletionQueue deletionQueue,
            IServiceScopeFactory serviceScopeFactory,
            int refreshIntervalInMinutes = 15) : base(deletionQueue, serviceScopeFactory, refreshIntervalInMinutes)
        {
        }

        protected override async Task InitializeQueueAsync(IUnitOfWork unitOfWork)
        {
            var allRecords = await unitOfWork.Cart.GetAllAsync();
            unitOfWork.Cart.RemoveRange(allRecords);
            await unitOfWork.SaveAsync();
        }

        protected override void Process(Cart entity, IUnitOfWork unitOfWork)
        {
            unitOfWork.Cart.Remove(entity);
        }

        protected override bool ShouldProcess(Cart entity, DateTime now)
        {
            return entity.ExpiresTo <= now;
        }
    }
}
