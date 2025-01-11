using DataAccess.Repository.IRepository;
using Microsoft.Extensions.DependencyInjection;
using Models.DatabaseRelatedModels;
using Utility.DiscountQueues.Interfaces;

public class DiscountDeletionService : DiscountBackgroundService
{
    public DiscountDeletionService(
        IDeletionDiscountQueue deletionQueue,
        IServiceScopeFactory serviceScopeFactory)
        : base(deletionQueue, serviceScopeFactory)
    {
    }

    protected override async Task InitializeQueueAsync(IServiceProvider serviceProvider)
    {
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
        var discounts = await unitOfWork.Discount.GetAllAsync();
        foreach (var discount in discounts)
        {
            await DiscoutQueue.EnqueueAsync(discount);
        }
    }

    protected override bool ShouldProcess(Discount discount, DateTime now)
    {
        return discount.EndTime <= now;
    }

    protected override async Task ProcessDiscountAsync(Discount discount, IServiceProvider serviceProvider)
    {
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
        unitOfWork.Discount.Remove(discount);

        await unitOfWork.SaveAsync();
    }
}