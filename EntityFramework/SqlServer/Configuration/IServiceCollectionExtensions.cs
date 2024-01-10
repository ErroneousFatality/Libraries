using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AndrejKrizan.EntityFramework.SqlServer.Configuration;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddSqlServerDbContext<TDbContext>(this IServiceCollection services,
        SqlServerSettings settings,
        ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
        ServiceLifetime optionsLifetime = ServiceLifetime.Scoped
    )
        where TDbContext : DbContext
        => services.AddDbContext<TDbContext>(context
            => context.UseSqlServer(settings.ConnectionString, sqlServer => sqlServer
                .UseQuerySplittingBehavior(settings.QuerySplitting
                    ? QuerySplittingBehavior.SplitQuery
                    : QuerySplittingBehavior.SingleQuery
                )
            ),
            contextLifetime,
            optionsLifetime
        );
}
