using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;

using AndrejKrizan.DotNet.Utilities;
using AndrejKrizan.DotNet.ValueObjects.PropertyNavigations;
using AndrejKrizan.EntityFramework.Common.Extensions.IQueryables;

using Microsoft.EntityFrameworkCore;

namespace AndrejKrizan.EntityFramework.Common.Repositories.Key;

public abstract class KeyRepository<TEntity, TKey> : Repository<TEntity>, IKeyRepository<TEntity, TKey>
    where TEntity : class
    where TKey: struct
{
    // Constructors
    protected KeyRepository(
        DbContext dbContext
    )
        : base(dbContext) { }

    // Methods
    public async Task<bool> ExistsAsync(TKey key, CancellationToken cancellationToken = default)
        => await DbSet.AnyAsync(KeyProperty.ToEqualsLambda(key), cancellationToken);

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
            keysContain = Expression.Call(instance: Expression.Constant(keys), method: containsMethodInfo, KeyProperty.Expression);
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
            keysContain = Expression.Call(instance: null, method: containsMethodInfo, KeyProperty.Expression);
        }
        Expression<Func<TEntity, bool>> keysContainEntityKeyLambda = Expression.Lambda<Func<TEntity, bool>>(keysContain, KeyProperty.Parameter);
        ImmutableArray<TEntity> entities = await DbSet
            .Where(keysContainEntityKeyLambda)
            .ToImmutableArrayAsync(cancellationToken);
        return entities;
    }

    public async Task<ImmutableArray<TKey>> GetKeysAsync(IEnumerable<TKey> keys, CancellationToken cancellationToken = default)
        => await DbSet
            .Select(KeyProperty.Lambda)
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
        TEntity mockEntity = KeyProperty.ToMockObject(key);
        DbSet.Attach(mockEntity);
        return mockEntity;
    }

    // Fields
    protected abstract Expression<Func<TEntity, TKey>> KeyLambda();
    protected internal static readonly PropertyNavigation<TEntity, TKey> KeyProperty = new(Utils.GetFromDefaultInstance((KeyRepository<TEntity, TKey> e) => e.KeyLambda()));
}