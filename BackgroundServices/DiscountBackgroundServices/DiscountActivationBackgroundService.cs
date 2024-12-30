using ControllersServices.ProductManagement.Interfaces;
using DataAccess.Repository.IRepository;
using Microsoft.Extensions.DependencyInjection;
using Models.DatabaseRelatedModels;
using Utility.DiscountQueues.Interfaces;

public class DiscountActivationService : DiscountBackgroundService
{
    public DiscountActivationService(
        IActivationDiscountQueue activationQueue,
        IServiceScopeFactory serviceScopeFactory)
        : base(activationQueue, serviceScopeFactory)
    {
    }

    protected override async Task InitializeQueueAsync(IServiceProvider serviceProvider)
    {
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
        var discounts = await unitOfWork.Discount.GetAllAsync(d => !d.isActive);
        foreach (var discount in discounts)
        {
            DiscoutQueue.Enqueue(discount);
        }
    }

    protected override bool ShouldProcess(Discount discount, DateTime now)
    {
        return discount.StartTime <= now;
    }

    protected override async Task ProcessDiscountAsync(Discount discount, IServiceProvider serviceProvider)
    {
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();

        discount.isActive = true;
        unitOfWork.Discount.Update(discount);
        await unitOfWork.SaveAsync();
    }
}