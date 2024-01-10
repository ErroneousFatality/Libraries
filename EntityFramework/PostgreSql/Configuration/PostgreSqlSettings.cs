namespace AndrejKrizan.EntityFramework.PostgreSql.Configuration;

public class PostgreSqlSettings
{
    public required string ConnectionString { get; init; }
    public bool QuerySplitting { get; init; }
}
