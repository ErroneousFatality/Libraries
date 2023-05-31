using AndrejKrizan.EntityFramework.PostgreSql.Settings;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AndrejKrizan.EntityFramework.PostgreSql.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPostgreSqlDbContext<TDbContext>(
        this IServiceCollection services,
        DatabaseSettings databaseSettings
    )
        where TDbContext : DbContext
        => services.AddDbContext<TDbContext>(options => options.UseNpgsql(databaseSettings.ConnectionString));
}
