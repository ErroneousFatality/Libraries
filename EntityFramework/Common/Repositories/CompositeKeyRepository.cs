using System.Collections.Immutable;
using System.Linq.Expressions;

using AndrejKrizan.DotNet.Extensions;
using AndrejKrizan.DotNet.ValueObjects.Keys;
using AndrejKrizan.EntityFramework.Common.Extensions.IQueryables;

using Microsoft.EntityFrameworkCore;

namespace AndrejKrizan.EntityFramework.Common.Repositories;

public class CompositeKeyRepository<TEntity, TKey> : Repository<TEntity>, IKeyRepository<TEntity, TKey>
    where TEntity : class
    where TKey : Key<TEntity, TKey>
{
    // Constructors
    public CompositeKeyRepository(DbContext dbContext)
        : base(dbContext) { }

    // Methods

    public async Task<bool> ExistsAsync(TKey key, CancellationToken cancellationToken = default)
        => await DbSet.AnyAsync(key.ToPredicateLambda(), cancellationToken);

    public async Task<TEntity?> GetAsync(TKey key, CancellationToken cancellationToken = default)
        => await DbSet.FindAsync(key.ToValues(), cancellationToken);


    public async Task<ImmutableArray<TEntity>> GetManyAsync(IEnumerable<TKey> keys, CancellationToken cancellationToken = default)
        => await DbSet
            .WhereAny(keys, key => key.ToPredicateLambda())
            .ToImmutableArrayAsync(cancellationToken);

    public async Task<ImmutableArray<TEntity>> GetManyAsync(IEnumerable<TKey> keys, int chunkSize, CancellationToken cancellationToken = default)
        => await DbSet.WhereAnyAsync(keys, chunkSize, key => key.ToPredicateLambda(), cancellationToken);


    public async Task<ImmutableArray<TKey>> GetKeysAsync(IEnumerable<TKey> keys, CancellationToken cancellationToken = default)
    {
        if (!keys.Any())
        {
            return ImmutableArray<TKey>.Empty;
        }
        Expression<Func<TEntity, TKey>> entityToKeyLambda = keys.First().ToKeyLambda();
        ImmutableArray<TKey> existingKeys = await DbSet
            .WhereAny(keys, key => key.ToPredicateLambda())
            .Select(entityToKeyLambda)
            .ToImmutableArrayAsync(cancellationToken);
        return existingKeys;
    }

    public async Task<ImmutableArray<TKey>> GetExistingKeysAsync(IEnumerable<TKey> keys, int chunkSize, CancellationToken cancellationToken = default)
    {
        if (!keys.Any())
        {
            return ImmutableArray<TKey>.Empty;
        }
        Expression<Func<TEntity, TKey>> entityToKeyLambda = keys.First().ToKeyLambda();
        ImmutableArray<TKey> existingKeys = await DbSet.WhereAnyAsync(keys, chunkSize, 
            key => key.ToPredicateLambda(), 
            query => query.Select(entityToKeyLambda), 
            cancellationToken
        );
        return existingKeys;
    }


    public void Delete(TKey key)
    {
        TEntity mockEntity = Mock(key);
        DbSet.Remove(mockEntity);
    }


    public void DeleteMany(params TKey[] keys)
        => DeleteMany((IEnumerable<TKey>)keys);

    public void DeleteMany(IEnumerable<TKey> keys)
    {
        IEnumerable<TEntity> mockEntities = keys.Select(Mock);
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
    protected TEntity Mock(TKey key)
    {
        TEntity mockEntity = key.ToObject();
        DbSet.Attach(mockEntity);
        return mockEntity;
    }
}
