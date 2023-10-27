using System.Collections.Immutable;
using AndrejKrizan.DotNet.CompositeKeys;
using AndrejKrizan.DotNet.CompositeKeys.KeyPropertyBindings;
using AndrejKrizan.DotNet.Extensions;
using AndrejKrizan.DotNet.Utilities;
using AndrejKrizan.EntityFramework.Common.Extensions.IQueryables;

using Microsoft.EntityFrameworkCore;

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
        => await DbSet.AnyAsync(key.ToEntityHasKeyLambda(), cancellationToken);

    public async Task<TEntity?> GetAsync(TKey key, CancellationToken cancellationToken = default)
    {
        object?[] keyValues = ICompositeKey<TEntity, TKey>.PropertyBindings.Select(binding => binding.KeyNavigation.GetValue(key)).ToArray();
        TEntity? entity = await DbSet.FindAsync(keyValues, cancellationToken);
        return entity;
    }


    public async Task<ImmutableArray<TEntity>> GetManyAsync(IEnumerable<TKey> keys, CancellationToken cancellationToken = default)
    => await DbSet
            .WhereAny(keys, key => key.ToEntityHasKeyLambda())
            .ToImmutableArrayAsync(cancellationToken);

    public async Task<ImmutableArray<TEntity>> GetManyAsync(IEnumerable<TKey> keys, int chunkSize, CancellationToken cancellationToken = default)
        => await DbSet.WhereAnyAsync(keys, chunkSize, key => key.ToEntityHasKeyLambda(), cancellationToken);


    public async Task<ImmutableArray<TKey>> GetKeysAsync(IEnumerable<TKey> keys, CancellationToken cancellationToken = default)
        => await DbSet
            .WhereAny(keys, key => key.ToEntityHasKeyLambda())
            .Select(TKey.Selector)
            .ToImmutableArrayAsync(cancellationToken);

    public async Task<ImmutableArray<TKey>> GetKeysAsync(IEnumerable<TKey> keys, int chunkSize, CancellationToken cancellationToken = default)
        => await DbSet.WhereAnyAsync(keys, chunkSize,
                key => key.ToEntityHasKeyLambda(),
                query => query.Select(TKey.Selector),
                cancellationToken
            );

    public void Delete(TKey key)
    {
        TEntity mockEntity = MockEntity(key);
        DbSet.Remove(mockEntity);
    }


    public void DeleteMany(params TKey[] keys)
        => DeleteMany((IEnumerable<TKey>)keys);

    public void DeleteMany(IEnumerable<TKey> keys)
    {
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

    // Protected methods
    protected TEntity MockEntity(TKey key)
    {
        TEntity entity = Utils.CreateDefaultInstance<TEntity>();
        foreach (IKeyPropertyBinding<TEntity, TKey> keyProperty in ICompositeKey<TEntity, TKey>.PropertyBindings)
        {
            keyProperty.SetValue(entity, key);
        }
        DbSet.Attach(entity);
        return entity;
    }
}
