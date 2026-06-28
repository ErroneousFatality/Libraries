using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AndrejKrizan.Hosting.Services.Periodic;

public class PeriodicServiceHost<TService> : BackgroundService
    where TService : IService
{
    // Fields
    private readonly IServiceScopeFactory ServiceScopeFactory;
    private readonly ILogger<PeriodicServiceHost<TService>> Logger;
    public readonly TimeSpan Period;

    // Constructors
    public PeriodicServiceHost(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<PeriodicServiceHost<TService>> logger,
        TimeSpan period
    )
    {
        ServiceScopeFactory = serviceScopeFactory;
        Logger = logger;
        Period = period;
    }

    // Methods
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested) { return; }
        try
        {
            using (PeriodicTimer timer = new(Period))
            {
                do
                {
                    try
                    {
                        await using (AsyncServiceScope asyncServiceScope = ServiceScopeFactory.CreateAsyncScope())
                        {
                            TService service = asyncServiceScope.ServiceProvider.GetRequiredService<TService>();
                            try
                            {
                                await service.ExecuteAsync(stoppingToken);
                            }
                            catch (Exception exception) when (exception is not OperationCanceledException)
                            {
                                Logger.LogError(exception, "Service {serviceTypeName} failed to execute.", typeof(TService).Name);
                            }
                        }
                    }
                    catch (Exception exception) when (exception is not OperationCanceledException)
                    {
                        Logger.LogCritical(exception, "Failed to host service {serviceTypeName}.", typeof(TService).Name);
                    }
                } while (await timer.WaitForNextTickAsync(stoppingToken));
            }
        }
        catch (OperationCanceledException) { }
    }
}
