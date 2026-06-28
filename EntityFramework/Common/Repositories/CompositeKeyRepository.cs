using System.Collections.Immutable;

using AndrejKrizan.DotNet.Collections;
using AndrejKrizan.DotNet.CompositeKeys;
using AndrejKrizan.DotNet.Repositories;
using AndrejKrizan.DotNet.Utilities;
using AndrejKrizan.EntityFramework.Common.Queries;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AndrejKrizan.EntityFramework.Common.Repositories;

public abstract class CompositeKeyRepository<TEntity, TKey> : Repository<TEntity>, IKeyRepository<TEntity, TKey>
    where TEntity : class
    where TKey : CompositeKey<TEntity, TKey>, ICompositeKey<TEntity, TKey>
{
    // Constructors
    public CompositeKeyRepository(DbContext dbContext)
        : base(dbContext) { }

    // Methods

    public async Task<bool> ExistsAsync(TKey key, CancellationToken cancellationToken = default)
        => await DbSet.AnyAsync(key.ToEntityHasKeyPredicate(), cancellationToken);

    public async Task<TEntity?> GetAsync(TKey key, CancellationToken cancellationToken = default)
    {
        object?[] keyValues = key.GetValues();
        TEntity? entity = await DbSet.FindAsync(keyValues, cancellationToken);
        return entity;
    }


    public async Task<ImmutableArray<TEntity>> GetManyAsync<TKeyCollection>(TKeyCollection keys, CancellationToken cancellationToken = default)
        where TKeyCollection: IReadOnlyCollection<TKey>
    {
        if (keys.Count == 0) { return []; }
        ImmutableArray<TEntity> entities = await DbSet
            .WhereAny(keys, key => key.ToEntityHasKeyPredicate())
            .ToImmutableArrayAsync(keys.Count, cancellationToken);
        return entities;
    }

    public async Task<ImmutableArray<TEntity>> GetManyAsync(IEnumerable<TKey> keys, CancellationToken cancellationToken = default)
        => keys is ImmutableArray<TKey> immutableArray ? await GetManyAsync(immutableArray, cancellationToken)
            : keys is IReadOnlyCollection<TKey> collection ? await GetManyAsync(collection, cancellationToken)
                : await GetManyAsync(keys.ToImmutableArray(), cancellationToken);


    public async Task<ImmutableArray<TEntity>> GetManyAsync<TKeyCollection>(TKeyCollection keys, int chunkSize, CancellationToken cancellationToken = default)
        where TKeyCollection : IReadOnlyCollection<TKey>
    {
        if (keys.Count == 0) { return []; }
        ImmutableArray<TEntity> entities = await DbSet.WhereAnyAsync(keys, chunkSize, key => key.ToEntityHasKeyPredicate(), cancellationToken);
        return entities;
    }

    public async Task<ImmutableArray<TEntity>> GetManyAsync(IEnumerable<TKey> keys, int chunkSize, CancellationToken cancellationToken = default)
        => keys is ImmutableArray<TKey> immutableArray ? await GetManyAsync(immutableArray, chunkSize, cancellationToken)
            : keys is IReadOnlyCollection<TKey> collection ? await GetManyAsync(collection, chunkSize, cancellationToken)
                : await GetManyAsync(keys.ToImmutableArray(), chunkSize, cancellationToken);



    public async Task<ImmutableArray<TKey>> GetKeysAsync<TKeyCollection>(TKeyCollection keys, CancellationToken cancellationToken = default)
        where TKeyCollection: IReadOnlyCollection<TKey>
    {
        if (keys.Count == 0) { return []; }
        ImmutableArray<TKey> _keys = await DbSet
            .WhereAny(keys, key => key.ToEntityHasKeyPredicate())
            .Select(TKey.Selector)
            .ToImmutableArrayAsync(keys.Count, cancellationToken);
        return _keys;
    }

    public async Task<ImmutableArray<TKey>> GetKeysAsync(IEnumerable<TKey> keys, CancellationToken cancellationToken = default)
        => keys is ImmutableArray<TKey> immutableArray ? await GetKeysAsync(immutableArray, cancellationToken)
            : keys is IReadOnlyCollection<TKey> collection ? await GetKeysAsync(collection, cancellationToken)
                : await GetKeysAsync(keys.ToImmutableArray(), cancellationToken);


    public async Task<ImmutableArray<TKey>> GetKeysAsync<TKeyCollection>(TKeyCollection keys, int chunkSize, CancellationToken cancellationToken = default)
        where TKeyCollection: IReadOnlyCollection<TKey>
    {
        if (keys.Count == 0) { return []; }
        ImmutableArray<TKey> _keys = await DbSet.WhereAnyAsync(keys, chunkSize,
                key => key.ToEntityHasKeyPredicate(),
                query => query.Select(TKey.Selector),
                cancellationToken
            );
        return _keys;
    }

    public async Task<ImmutableArray<TKey>> GetKeysAsync(IEnumerable<TKey> keys, int chunkSize, CancellationToken cancellationToken = default)
        => keys is ImmutableArray<TKey> immutableArray ? await GetKeysAsync(immutableArray, chunkSize, cancellationToken)
            : keys is IReadOnlyCollection<TKey> collection ? await GetKeysAsync(collection, chunkSize, cancellationToken)
                : await GetKeysAsync(keys.ToImmutableArray(), chunkSize, cancellationToken);


    /// <returns>Whether the key was found and deleted.</returns>
    public async Task<bool> DeleteAsync(TKey key, CancellationToken cancellationToken = default)
    {
        int deletedCount = await DbSet
            .Where(key.ToEntityHasKeyPredicate())
            .ExecuteDeleteAsync(cancellationToken);
        return deletedCount > 0;
    }

    /// <returns>Whether the entity was found and deleted by its key.</returns>
    public async Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        TKey key = CompositeKey<TEntity, TKey>.Create(entity);
        return await DeleteAsync(key, cancellationToken);
    }

    /// <returns>The number of keys found and deleted.</returns>
    public async Task<int> DeleteManyAsync(IEnumerable<TKey> keys, CancellationToken cancellationToken = default)
    {
        int deletedCount = await DbSet
            .WhereAny(keys, key => key.ToEntityHasKeyPredicate())
            .ExecuteDeleteAsync(cancellationToken);
        return deletedCount;
    }

    /// <returns>The number of keys found and deleted.</returns>
    public async Task<int> DeleteManyAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        ImmutableArray<TKey> keys = entities.Convert(CompositeKey<TEntity, TKey>.Create);
        return await DeleteManyAsync(keys, cancellationToken);
    }

    public void Delete(TKey key)
    {
        TEntity mockEntity = MockEntity(key);
        DbSet.Remove(mockEntity);
    }

    public void DeleteMany(IEnumerable<TKey> keys)
    {
        if (!keys.Any())
        {
            return;
        }
        IEnumerable<TEntity> mockEntities = keys.Select(MockEntity);
        DbSet.RemoveRange(mockEntities);
    }

    public void Untrack(TKey key)
    {
        EntityEntry<TEntity>? entry = DbSet.Local.FindEntry(key)
            ?? throw new ArgumentException($"There is no tracked entity with key = {key}.", nameof(key));
        entry.State = EntityState.Detached;
    }


    /// <returns>whether key was found in the database before being deleted</returns>
    [Obsolete(message: $"Use the {nameof(DeleteAsync)} method instead.", error: true)]
    public async Task<bool> TryDeleteAsync(TKey key, CancellationToken cancellationToken = default)
    {
        if (!await ExistsAsync(key, cancellationToken))
        {
            return false;
        }
        Delete(key);
        return true;
    }

    [Obsolete(message: $"Use the {nameof(DeleteMany)} with an inline initialized collection instead. E.g.: [1, 2, 3]", error: true)]
    public void DeleteMany(params TKey[] keys)
        => DeleteMany((IEnumerable<TKey>)keys);

    // Protected methods
    protected TEntity MockEntity(TKey key)
    {
        TEntity entity = Utils.CreateDefaultInstance<TEntity>();
        key.InjectValuesInto(entity);
        DbSet.Attach(entity);
        return entity;
    }
}
