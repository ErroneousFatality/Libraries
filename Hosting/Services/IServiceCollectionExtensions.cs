using Microsoft.Extensions.DependencyInjection;

namespace AndrejKrizan.Hosting.Services;
public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddHostedService<TService>(this IServiceCollection services)
        where TService : class, IService
        => services
            .AddScoped<TService>()
            .AddHostedService<ServiceHost<TService>>();
            
}
