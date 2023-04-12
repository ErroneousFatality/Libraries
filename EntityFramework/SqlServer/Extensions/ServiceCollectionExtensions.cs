using AndrejKrizan.EntityFramework.SqlServer.Settings;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AndrejKrizan.EntityFramework.SqlServer.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSqlServerDbContext<TDbContext>(
        this IServiceCollection services,
        DatabaseSettings settings
    )
        where TDbContext : DbContext
    => services.AddDbContext<TDbContext>(dbContext
        => dbContext.UseSqlServer(settings.ConnectionString, sqlServer
            => sqlServer.UseQuerySplittingBehavior(
                settings.QuerySplitting
                    ? QuerySplittingBehavior.SplitQuery
                    : QuerySplittingBehavior.SingleQuery
            )
        ),
        contextLifetime: ServiceLifetime.Scoped,
        optionsLifetime: ServiceLifetime.Scoped
    );
}
