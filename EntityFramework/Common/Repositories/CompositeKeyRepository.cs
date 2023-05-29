using System.Collections.Immutable;
using System.Linq.Expressions;

using AndrejKrizan.DotNet.Entities;
using AndrejKrizan.DotNet.Extensions;
using AndrejKrizan.DotNet.Utilities;
using AndrejKrizan.DotNet.ValueObjects.PropertyBindings;
using AndrejKrizan.EntityFramework.Common.Extensions.IQueryables;

using Microsoft.EntityFrameworkCore;

namespace AndrejKrizan.EntityFramework.Common.Repositories;

public abstract class CompositeKeyRepository<TEntity, TKey> : Repository<TEntity>, IKeyRepository<TEntity, TKey>
    where TEntity : class
    where TKey : struct, IKey<TEntity, TKey>
{
    // Constructors
    public CompositeKeyRepository(DbContext dbContext)
        : base(dbContext) { }

    // Methods

    public async Task<bool> ExistsAsync(TKey key, CancellationToken cancellationToken = default)
        => await DbSet.AnyAsync(CreateKeyPredicateLambda(key), cancellationToken);

    public async Task<TEntity?> GetAsync(TKey key, CancellationToken cancellationToken = default)
    {
        object?[] keyValues = IKey<TEntity, TKey>.PropertyBindings.Select(binding => binding.KeyProperty.GetValue(key)).ToArray();
        TEntity? entity = await DbSet.FindAsync(keyValues, cancellationToken);
        return entity;
    }


    public async Task<ImmutableArray<TEntity>> GetManyAsync(IEnumerable<TKey> keys, CancellationToken cancellationToken = default)
        => await DbSet
            .WhereAny(keys, CreateKeyPredicateLambda)
            .ToImmutableArrayAsync(cancellationToken);

    public async Task<ImmutableArray<TEntity>> GetManyAsync(IEnumerable<TKey> keys, int chunkSize, CancellationToken cancellationToken = default)
        => await DbSet.WhereAnyAsync(keys, chunkSize, CreateKeyPredicateLambda, cancellationToken);


    public async Task<ImmutableArray<TKey>> GetKeysAsync(IEnumerable<TKey> keys, CancellationToken cancellationToken = default)
        => await DbSet
            .WhereAny(keys, CreateKeyPredicateLambda)
            .Select(TKey.Lambda)
            .ToImmutableArrayAsync(cancellationToken);

    public async Task<ImmutableArray<TKey>> GetKeysAsync(IEnumerable<TKey> keys, int chunkSize, CancellationToken cancellationToken = default)
        => await DbSet.WhereAnyAsync(keys, chunkSize,
                CreateKeyPredicateLambda,
                query => query.Select(TKey.Lambda),
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
    protected Expression<Func<TEntity, bool>> CreateKeyPredicateLambda(TKey key)
        => Expression.Lambda<Func<TEntity, bool>>(
            IKey<TEntity, TKey>.PropertyBindings
                .Select(binding => binding.ToEqualsExpression(key))
                .Aggregate(Expression.AndAlso),
            IKey<TEntity, TKey>.EntityParameter
        );

    protected TEntity MockEntity(TKey key)
    {
        TEntity entity = Utils.CreateDefaultInstance<TEntity>();
        foreach (IPropertyBinding<TEntity, TKey> keyProperty in IKey<TEntity, TKey>.PropertyBindings)
        {
            keyProperty.SetValue(entity, key);
        }
        DbSet.Attach(entity);
        return entity;
    }
}
