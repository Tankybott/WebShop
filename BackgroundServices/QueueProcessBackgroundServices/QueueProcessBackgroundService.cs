using DataAccess.Repository.IRepository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Models.ContractInterfaces;

public abstract class QueueProcessBackgroundService<TEntity> : BackgroundService where TEntity : IHasId
{
    protected readonly ISortedQueue<TEntity> _entityQueue;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly int _refreshIntervalInMinutes;
    private bool _isInitialized = false; 

    protected QueueProcessBackgroundService(ISortedQueue<TEntity> sortedQueue, IServiceScopeFactory serviceScopeFactory, int refreshIntervalInMinutes = 1)
    {
        _entityQueue = sortedQueue;
        _serviceScopeFactory = serviceScopeFactory;
        _refreshIntervalInMinutes = refreshIntervalInMinutes;
    }

    protected abstract Task InitializeQueueAsync(IUnitOfWork unitOfWork);
    protected abstract void Process(TEntity entity, IUnitOfWork unitOfWork);
    protected abstract bool ShouldProcess(TEntity entity, DateTime now);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var serviceProvider = scope.ServiceProvider;
            var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();

            if (!_isInitialized)
            {
                await InitializeQueueAsync(unitOfWork);
                _isInitialized = true;
            }
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.UtcNow;
            bool wasProcessed = false;

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();

                while (!_entityQueue.IsEmpty())
                {
                    var entity = await _entityQueue.PeekAsync();
                    if (entity == null || !ShouldProcess(entity, now))
                        break;

                    await _entityQueue.DequeueAsync();

                    Process(entity, unitOfWork);
                    wasProcessed = true;
                }

                if (wasProcessed)
                {
                    await unitOfWork.SaveAsync();
                }
            }

            await Task.Delay(TimeSpan.FromMinutes(_refreshIntervalInMinutes), stoppingToken);
        }
    }
}