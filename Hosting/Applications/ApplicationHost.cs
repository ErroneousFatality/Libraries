using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AndrejKrizan.Hosting.Applications;
public sealed class ApplicationHost : BackgroundService 
{
    // Properties
    private readonly IHostApplicationLifetime ApplicationHostLifetime;
    private readonly IServiceScopeFactory ServiceScopeFactory;
    private readonly ILogger<ApplicationHost> Logger;

    // Constructors
    public ApplicationHost(
        IHostApplicationLifetime applicationHostLifetime,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<ApplicationHost> logger
    )
    {
        ApplicationHostLifetime = applicationHostLifetime;
        ServiceScopeFactory = serviceScopeFactory;
        Logger = logger;
    }

    // Methods
    protected override async Task ExecuteAsync(CancellationToken stoppingToken = default)
    {
        if (stoppingToken.IsCancellationRequested)
        {
            Environment.ExitCode = (int)ExitCode.Success;
            return;
        }
        try
        {
            await using (AsyncServiceScope asyncServiceScope = ServiceScopeFactory.CreateAsyncScope())
            {
                IApplication application = asyncServiceScope.ServiceProvider.GetRequiredService<IApplication>();
                try
                {
                    await application.ExecuteAsync(stoppingToken);
                    Environment.ExitCode = (int)ExitCode.Success;
                }
                catch (OperationCanceledException)
                {
                    Environment.ExitCode = (int)ExitCode.Success;
                }
                catch (Exception exception)
                {
                    Logger.LogError(exception, "Application failed to execute.");
                    Environment.ExitCode = (int)ExitCode.Fail;
                }
            }
        }
        catch (Exception exception)
        {
            Logger.LogCritical(exception, "Failed to host application.");
            Environment.ExitCode = (int)ExitCode.Fail;
        }
        finally
        {
            ApplicationHostLifetime.StopApplication();
        }        
    }
}