using System.Collections.Immutable;

namespace AndrejKrizan.EntityFramework.Common.Repositories;
public interface IKeyRepository<TEntity, TKey>
    where TEntity : class
{
    Task<bool> ExistsAsync(TKey key, CancellationToken cancellationToken = default);

    Task<TEntity?> GetAsync(TKey key, CancellationToken cancellationToken = default);
    Task<ImmutableArray<TEntity>> GetManyAsync(IEnumerable<TKey> keys, CancellationToken cancellationToken = default);
    Task<ImmutableArray<TKey>> GetKeysAsync(IEnumerable<TKey> keys, CancellationToken cancellationToken = default);

    void Delete(TKey key);
    /// <returns>whether key was found in the database before being deleted</returns>
    Task<bool> TryDeleteAsync(TKey key, CancellationToken cancellationToken = default);

    
    void DeleteMany(IEnumerable<TKey> keys);
    void DeleteMany(params TKey[] keys);
}