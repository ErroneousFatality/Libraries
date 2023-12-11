using System.Collections.Immutable;
using System.Data;

using Microsoft.Data.SqlClient;

namespace AndrejKrizan.SqlClient;

public sealed class SqlClientRepository
{
    // Fields
    public readonly string ConnectionString;

    // Constructors
    public SqlClientRepository(string connectionString)
    {
        ConnectionString = connectionString;
    }

    // Methods

    public async Task<ImmutableArray<T>> GetManyAsync<T>(string query, Func<SqlDataReader, T> read, CancellationToken cancellationToken = default)
    {
        ImmutableArray<T>.Builder builder = ImmutableArray.CreateBuilder<T>();
        using (SqlConnection connection = new(ConnectionString))
        using (SqlCommand command = new(query, connection))
        {
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

    public async Task ExecuteAsync(string name,
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

    public async Task<T> ExecuteAndGetAsync<T>(string name,
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

    public async Task<ImmutableArray<T>> ExecuteAndGetManyAsync<T>(string name,
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
