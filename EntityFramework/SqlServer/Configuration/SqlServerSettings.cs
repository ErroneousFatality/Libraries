namespace AndrejKrizan.EntityFramework.SqlServer.Configuration;

public class SqlServerSettings
{
    public required string ConnectionString { get; init; }
    public bool QuerySplitting { get; init; }
}
