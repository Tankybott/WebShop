using DataAccess.Repository.IRepository;
using Microsoft.Extensions.DependencyInjection;
using Models.DatabaseRelatedModels;
using Utility.DiscountQueues.Interfaces;

public class DiscountActivationService : QueueProcessBackgroundService<Discount>
{
    public DiscountActivationService(
        IDiscountActivationQueue activationQueue,
        IServiceScopeFactory serviceScopeFactory)
        : base(activationQueue, serviceScopeFactory)
    {
    }

    protected override async Task InitializeQueueAsync(IUnitOfWork unitOfWork)
    {
        var discounts = await unitOfWork.Discount.GetAllAsync(d => !d.isActive);
        foreach (var discount in discounts)
        {
            await _entityQueue.EnqueueAsync(discount);
        }
    }

    protected override bool ShouldProcess(Discount discount, DateTime now)
    {
        return discount.StartTime <= now;
    }

    protected override void Process(Discount discount, IUnitOfWork unitOfWork)
    {
        discount.isActive = true;
        unitOfWork.Discount.Update(discount);
    }
}