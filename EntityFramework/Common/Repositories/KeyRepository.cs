using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;

using AndrejKrizan.DotNet.Utilities;
using AndrejKrizan.DotNet.ValueObjects.PropertyNavigations;
using AndrejKrizan.EntityFramework.Common.Extensions.IQueryables;

using Microsoft.EntityFrameworkCore;

namespace AndrejKrizan.EntityFramework.Common.Repositories;

public abstract class KeyRepository<TEntity, TKey> : Repository<TEntity>, IKeyRepository<TEntity, TKey> 
    where TEntity : class
    where TKey : struct
{
    // Constructors
    public KeyRepository(DbContext dbContext)
        : base(dbContext) { }

    // Methods

    public async Task<bool> ExistsAsync(TKey key, CancellationToken cancellationToken = default)
        => await DbSet.AnyAsync(KeyNavigation.ToEqualsLambda(key), cancellationToken);

    public async Task<TEntity?> GetAsync(TKey key, CancellationToken cancellationToken = default)
        => await DbSet.FindAsync(new object?[] { key }, cancellationToken);

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
        Expression<Func<TEntity, bool>> keysContainEntityKeyLambda = Expression.Lambda<Func<TEntity, bool>>(keysContain, EntityParameter);
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

    // Protected properties
    protected abstract Expression<Func<TEntity, TKey>> KeyLambda { get; }

    protected PropertyNavigation<TEntity, TKey> KeyNavigation => _keyNavigation ??= new(KeyLambda);
    private static PropertyNavigation<TEntity, TKey>? _keyNavigation;

    protected ParameterExpression EntityParameter => _entityParameter ??= KeyLambda.Parameters.Single();
    private static ParameterExpression? _entityParameter;

    // Protected methods
    protected virtual TEntity MockEntity(TKey key)
    {
        TEntity entity = Utils.CreateDefaultInstance<TEntity>();
        KeyNavigation.SetValue(entity, key);
        DbSet.Attach(entity);
        return entity;
    }
}
