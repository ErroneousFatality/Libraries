using Microsoft.Extensions.DependencyInjection;

namespace AndrejKrizan.Hosting.Applications;
public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddHostedApplication<TApplication>(this IServiceCollection services)
        where TApplication : class, IApplication
        => services
            .AddHostedService<ApplicationHost>()
            .AddScoped<IApplication, TApplication>();
}
