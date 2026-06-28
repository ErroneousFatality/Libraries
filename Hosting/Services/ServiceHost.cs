using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AndrejKrizan.Hosting.Services;
public class ServiceHost<TService> : BackgroundService
    where TService: IService
{
    // Fields
    private readonly IServiceScopeFactory ServiceScopeFactory;
    private readonly ILogger<ServiceHost<TService>> Logger;

    // Constructors
    public ServiceHost(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<ServiceHost<TService>> logger
    )
    {
        ServiceScopeFactory = serviceScopeFactory;
        Logger = logger;
    }

    // Methods
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested) { return; }
        try
        {
            await using (AsyncServiceScope asyncServiceScope = ServiceScopeFactory.CreateAsyncScope())
            {
                TService service = asyncServiceScope.ServiceProvider.GetRequiredService<TService>();
                try
                {
                    await service.ExecuteAsync(stoppingToken);
                }
                catch (OperationCanceledException) { }
                catch (Exception exception)
                {
                    Logger.LogError(exception, "Service {serviceTypeName} failed to execute.", typeof(TService).ToString());
                }
            }
        }
        catch (Exception exception)
        {
            Logger.LogCritical(exception, "Failed to host service {serviceTypeName}.", typeof(TService).ToString());
        }
    }
}
