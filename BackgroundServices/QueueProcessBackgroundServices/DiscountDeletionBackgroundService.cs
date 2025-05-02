using DataAccess.Repository.IRepository;
using Microsoft.Extensions.DependencyInjection;
using Models.DatabaseRelatedModels;
using Utility.DiscountQueues.Interfaces;

public class DiscountDeletionService : QueueProcessBackgroundService<Discount>
{
    public DiscountDeletionService(
        IDiscountDeletionQueue deletionQueue,
        IServiceScopeFactory serviceScopeFactory)
        : base(deletionQueue, serviceScopeFactory)
    {
    }

    protected override async Task InitializeQueueAsync(IUnitOfWork unitOfWork)
    {
        var discounts = await unitOfWork.Discount.GetAllAsync();
        foreach (var discount in discounts)
        {
            await _entityQueue.EnqueueAsync(discount);
        }
    }

    protected override bool ShouldProcess(Discount discount, DateTime now)
    {
        return discount.EndTime <= now;
    }

    protected override void Process(Discount discount, IUnitOfWork unitOfWork)
    {
        unitOfWork.Discount.Remove(discount);
    }
}