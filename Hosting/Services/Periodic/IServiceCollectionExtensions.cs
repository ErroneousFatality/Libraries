using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AndrejKrizan.Hosting.Services.Periodic;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddHostedPeriodicService<TService>(this IServiceCollection services, TimeSpan period)
        where TService : class, IService
        => services
            .AddScoped<TService>()
            .AddHostedService(serviceProvider
                => new PeriodicServiceHost<TService>(
                    serviceScopeFactory: serviceProvider.GetRequiredService<IServiceScopeFactory>(),
                    logger: serviceProvider.GetRequiredService<ILogger<PeriodicServiceHost<TService>>>(),
                    period
                )
            );
}
