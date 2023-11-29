using System.Collections.Immutable;
using System.Data;

using Microsoft.Data.SqlClient;

namespace AndrejKrizan.SqlClient;
internal abstract class SqlClientRepository
{
    // Static properties
    public static string ConnectionString { get; private set; } = null!;

    // Static methods
    internal static void Initialize(string connectionString)
    {
        ConnectionString = connectionString;
    }

    // Protected methods

    protected internal static async Task ExecuteAsync(string name,
        Action<SqlParameterCollection> configureParameters,
        CancellationToken cancellationToken = default
    )
    {
        using SqlConnection connection = new(ConnectionString);
        using SqlCommand command = new(name, connection) { CommandType = CommandType.StoredProcedure };
        configureParameters(command.Parameters);
        await connection.OpenAsync(cancellationToken);
        _ = await command.ExecuteNonQueryAsync(cancellationToken);
    }

    protected internal static async Task<T> ExecuteAndGetAsync<T>(string name,
        Action<SqlParameterCollection> configureParameters,
        Func<SqlParameterCollection, T> select,
        CancellationToken cancellationToken = default
    )
    {
        T result;
        using (SqlConnection connection = new(ConnectionString))
        using (SqlCommand command = new(name, connection) { CommandType = CommandType.StoredProcedure })
        {
            configureParameters(command.Parameters);
            await connection.OpenAsync(cancellationToken);
            _ = await command.ExecuteNonQueryAsync(cancellationToken);
            result = select(command.Parameters);
        }
        return result;
    }

    protected internal static async Task<ImmutableArray<T>> ExecuteAndGetManyAsync<T>(string name,
        Action<SqlParameterCollection> configureParameters,
        Func<SqlDataReader, T> read,
        CancellationToken cancellationToken = default
    )
    {
        ImmutableArray<T>.Builder builder = ImmutableArray.CreateBuilder<T>();
        using (SqlConnection connection = new(ConnectionString))
        using (SqlCommand command = new(name, connection) { CommandType = CommandType.StoredProcedure })
        {
            configureParameters(command.Parameters);
            await connection.OpenAsync(cancellationToken);
            SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                T result = read(reader);
                builder.Add(result);
            }
        }
        ImmutableArray<T> results = builder.ToImmutableArray();
        return results;
    }
}
