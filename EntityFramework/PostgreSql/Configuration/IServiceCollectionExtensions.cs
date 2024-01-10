using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AndrejKrizan.EntityFramework.PostgreSql.Configuration;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddPostgreSqlDbContext<TDbContext>(this IServiceCollection services,
        PostgreSqlSettings settings,
        ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
        ServiceLifetime optionsLifetime = ServiceLifetime.Scoped

    )
        where TDbContext : DbContext
        => services.AddDbContext<TDbContext>(context
            => context.UseNpgsql(settings.ConnectionString, npgsql
                => npgsql.UseQuerySplittingBehavior(
                    settings.QuerySplitting
                        ? QuerySplittingBehavior.SplitQuery
                        : QuerySplittingBehavior.SingleQuery
                )
            ),
            contextLifetime,
            optionsLifetime
        );
}
