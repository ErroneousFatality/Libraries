using System.Collections.Immutable;

using AndrejKrizan.DotNet.CompositeKeys;
using AndrejKrizan.DotNet.Repositories;
using AndrejKrizan.DotNet.Utilities;
using AndrejKrizan.EntityFramework.Common.Queries;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AndrejKrizan.EntityFramework.Common.Repositories;

public abstract class CompositeKeyRepository<TEntity, TKey> : Repository<TEntity>, IKeyRepository<TEntity, TKey>
    where TEntity : class
    where TKey : ICompositeKey<TEntity, TKey>
{
    // Constructors
    public CompositeKeyRepository(DbContext dbContext)
        : base(dbContext) { }

    // Methods

    public async Task<bool> ExistsAsync(TKey key, CancellationToken cancellationToken = default)
        => await DbSet.AnyAsync(key.ToEntityPredicate(), cancellationToken);

    public async Task<TEntity?> GetAsync(TKey key, CancellationToken cancellationToken = default)
    {
        object?[] keyValues = key.GetValues();
        TEntity? entity = await DbSet.FindAsync(keyValues, cancellationToken);
        return entity;
    }


    public async Task<ImmutableArray<TEntity>> GetManyAsync(IEnumerable<TKey> keys, CancellationToken cancellationToken = default)
    {
        if (!keys.Any())
        {
            return [];
        }
        ImmutableArray<TEntity> entities = await DbSet
            .WhereAny(keys, key => key.ToEntityPredicate())
            .ToImmutableArrayAsync(cancellationToken);
        return entities;
    }
        

    public async Task<ImmutableArray<TEntity>> GetManyAsync(IEnumerable<TKey> keys, int chunkSize, CancellationToken cancellationToken = default)
    {
        if (!keys.Any())
        {
            return [];
        }
        ImmutableArray<TEntity> entities = await DbSet.WhereAnyAsync(keys, chunkSize, key => key.ToEntityPredicate(), cancellationToken);
        return entities;
    }


    public async Task<ImmutableArray<TKey>> GetKeysAsync(IEnumerable<TKey> keys, CancellationToken cancellationToken = default)
    {
        if (!keys.Any())
        {
            return [];
        }
        ImmutableArray<TKey> _keys = await DbSet
            .WhereAny(keys, key => key.ToEntityPredicate())
            .Select(TKey.Selector)
            .ToImmutableArrayAsync(cancellationToken);
        return _keys;
    }



    public async Task<ImmutableArray<TKey>> GetKeysAsync(IEnumerable<TKey> keys, int chunkSize, CancellationToken cancellationToken = default)
    {
        if (!keys.Any())
        {
            return [];
        }
        ImmutableArray<TKey> _keys = await DbSet.WhereAnyAsync(keys, chunkSize,
                key => key.ToEntityPredicate(),
                query => query.Select(TKey.Selector),
                cancellationToken
            );
        return _keys;
    }

    public void Delete(TKey key)
    {
        TEntity mockEntity = MockEntity(key);
        DbSet.Remove(mockEntity);
    }


    public void DeleteMany(params TKey[] keys)
        => DeleteMany((IEnumerable<TKey>)keys);

    public void DeleteMany(IEnumerable<TKey> keys)
    {
        if (!keys.Any())
        {
            return;
        }
        IEnumerable<TEntity> mockEntities = keys.Select(MockEntity);
        DbSet.RemoveRange(mockEntities);
    }


    /// <returns>whether key was found in the database before being deleted</returns>
    public async Task<bool> TryDeleteAsync(TKey key, CancellationToken cancellationToken = default)
    {
        if (!await ExistsAsync(key, cancellationToken))
        {
            return false;
        }
        Delete(key);
        return true;
    }

    public void Untrack(TKey key)
    {
        EntityEntry<TEntity>? entry = DbSet.Local.FindEntry(key)
            ?? throw new ArgumentException($"There is no tracked entity with key = {key}.", nameof(key));
        entry.State = EntityState.Detached;
    }


    // Protected methods
    protected TEntity MockEntity(TKey key)
    {
        TEntity entity = Utils.CreateDefaultInstance<TEntity>();
        key.InjectValuesInto(entity);
        DbSet.Attach(entity);
        return entity;
    }
}
