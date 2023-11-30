using Microsoft.Extensions.DependencyInjection;

namespace AndrejKrizan.SqlClient.Configuration;
public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddSqlClientRepository(this IServiceCollection services, string connectionString)
        => services.AddTransient<SqlClientRepository>(_ => new SqlClientRepository(connectionString));
}
