using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Models.DatabaseRelatedModels;

public abstract class DiscountBackgroundService : BackgroundService
{
    public IQueueBase<Discount> DiscoutQueue { get; set; }
    private readonly IServiceScopeFactory _serviceScopeFactory;

    protected DiscountBackgroundService(IQueueBase<Discount> discountQueue, IServiceScopeFactory serviceScopeFactory)
    {
        DiscoutQueue = discountQueue;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected abstract Task InitializeQueueAsync(IServiceProvider serviceProvider);
    protected abstract Task ProcessDiscountAsync(Discount discount, IServiceProvider serviceProvider);
    protected abstract bool ShouldProcess(Discount discount, DateTime now);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var serviceProvider = scope.ServiceProvider;
            await InitializeQueueAsync(serviceProvider);
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;

            while (!DiscoutQueue.IsEmpty)
            {
                var discount = DiscoutQueue.Peek();
                if (discount == null || !ShouldProcess(discount, now))
                    break;

                DiscoutQueue.Dequeue();

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var serviceProvider = scope.ServiceProvider;
                    await ProcessDiscountAsync(discount, serviceProvider);
                }
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}