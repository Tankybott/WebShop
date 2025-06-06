using BackgroundServices.BackgroundProcessors;
using Microsoft.Extensions.Hosting;

public abstract class ProcessBackgroundService : BackgroundService
{ 
    private readonly int _intervalMinutes;

    public ProcessBackgroundService( int intervalMinutes)
    {
        _intervalMinutes = intervalMinutes;
    }

    protected abstract Task ProcessAsync();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessAsync();
            await Task.Delay(TimeSpan.FromMinutes(_intervalMinutes), stoppingToken);
        }
    }
}