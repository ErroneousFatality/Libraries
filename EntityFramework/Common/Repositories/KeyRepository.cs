using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;

using AndrejKrizan.DotNet;
using AndrejKrizan.DotNet.ValueObjects;
using AndrejKrizan.EntityFramework.Common.Extensions.IQueryables;

using Microsoft.EntityFrameworkCore;

namespace AndrejKrizan.EntityFramework.Common.Repositories;

public class KeyRepository<TEntity, TKey> : Repository<TEntity>, IKeyRepository<TEntity, TKey> 
    where TEntity : class
{
    // Fields
    protected internal readonly PropertyNavigation<TEntity, TKey> KeyNavigation;

    // Constructors
    public KeyRepository(
        DbContext dbContext,
        Expression<Func<TEntity, TKey>> keyPropertyLambda
    )
        : base(dbContext)
    {
        KeyNavigation = new(keyPropertyLambda);
    }

    // Methods
    public async Task<bool> ExistsAsync(TKey key, CancellationToken cancellationToken = default)
        => await DbSet.AnyAsync(KeyNavigation.ToEqualsLambda(key), cancellationToken);

    public async Task<TEntity?> GetAsync(TKey key, CancellationToken cancellationToken = default)
        => await DbSet.FindAsync(keyValues: new object?[] { key }, cancellationToken);

    public async Task<ImmutableArray<TEntity>> GetManyAsync(IEnumerable<TKey> keys, CancellationToken cancellationToken = default)
    {
        if (!keys.Any())
        {
            return ImmutableArray<TEntity>.Empty;
        }

        MethodCallExpression keysContain;
        if (keys is ImmutableArray<TKey>)
        {
            MethodInfo containsMethodInfo = typeof(ImmutableArray<TKey>)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Single(method
                    => method.Name == nameof(ImmutableArray<TKey>.Contains)
                    && method.GetParameters().Length == 1
                );
            keysContain = Expression.Call(instance: Expression.Constant(keys), method: containsMethodInfo, KeyNavigation.Expression);
        }
        else
        {
            MethodInfo containsMethodInfo = typeof(Enumerable)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Single(method
                    => method.Name == nameof(Enumerable.Contains)
                    && method.GetParameters().Length == 2
                )
                .MakeGenericMethod(typeof(TKey));
            keysContain = Expression.Call(instance: null, method: containsMethodInfo, KeyNavigation.Expression);
        }
        Expression<Func<TEntity, bool>> keysContainEntityKeyLambda = Expression.Lambda<Func<TEntity, bool>>(keysContain, KeyNavigation.Parameter);
        ImmutableArray<TEntity> entities = await DbSet
            .Where(keysContainEntityKeyLambda)
            .ToImmutableArrayAsync(cancellationToken);
        return entities;
    }

    public async Task<ImmutableArray<TKey>> GetKeysAsync(IEnumerable<TKey> keys, CancellationToken cancellationToken = default)
        => await DbSet
            .Select(KeyNavigation.Lambda)
            .Where(key => keys.Contains(key))
            .ToImmutableArrayAsync(cancellationToken);
    public Task<ImmutableArray<TKey>> GetIdsAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default)
        => GetKeysAsync(ids, cancellationToken);

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
        TEntity mockEntity = KeyNavigation.ToMockObject(key);
        DbSet.Attach(mockEntity);
        return mockEntity;
    }
}

public class KeyRepository<TEntity> : KeyRepository<TEntity, Guid>
    where TEntity : Entity
{
    // Constructors
    public KeyRepository(DbContext dbContext)
        : base(dbContext, entity => entity.Id)
    {
    }
}
