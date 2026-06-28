using System.Collections.Immutable;

namespace AndrejKrizan.DotNet.Repositories;
public interface IKeyRepository<TEntity, TKey> : IRepository<TEntity>
    where TEntity : class
{
    Task<bool> ExistsAsync(TKey key, CancellationToken cancellationToken = default);

    Task<TEntity?> GetAsync(TKey key, CancellationToken cancellationToken = default);

    Task<ImmutableArray<TEntity>> GetManyAsync<TKeyCollection>(TKeyCollection keys, CancellationToken cancellationToken = default) where TKeyCollection: IReadOnlyCollection<TKey>;
    Task<ImmutableArray<TEntity>> GetManyAsync(IEnumerable<TKey> keys, CancellationToken cancellationToken = default);

    Task<ImmutableArray<TKey>> GetKeysAsync<TKeyCollection>(TKeyCollection keys, CancellationToken cancellationToken = default) where TKeyCollection : IReadOnlyCollection<TKey>;
    Task<ImmutableArray<TKey>> GetKeysAsync(IEnumerable<TKey> keys, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(TKey key, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<int> DeleteManyAsync(IEnumerable<TKey> keys, CancellationToken cancellationToken = default);
    Task<int> DeleteManyAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    void Delete(TKey key);
    void DeleteMany(IEnumerable<TKey> keys);
    
    void Untrack(TKey key);

    /// <returns>whether key was found in the database before being deleted</returns>
    [Obsolete(message: $"Use the {nameof(DeleteAsync)} method instead.", error: true)]
    Task<bool> TryDeleteAsync(TKey key, CancellationToken cancellationToken = default);

    [Obsolete(message: $"Use the {nameof(DeleteMany)} with an inline initialized collection instead. E.g.: [1, 2, 3]", error: true)]
    void DeleteMany(params TKey[] keys);
}