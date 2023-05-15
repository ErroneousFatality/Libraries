using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;

using AndrejKrizan.DotNet;
using AndrejKrizan.EntityFramework.Common.Extensions.IQueryables;
using AndrejKrizan.EntityFramework.Common.Extensions.Lambda;

using Microsoft.EntityFrameworkCore;

namespace AndrejKrizan.EntityFramework.Common.Repositories;

public class KeyRepository<TEntity, TKey> : Repository<TEntity>
    where TEntity : class
{
    // Fields
    private PropertyInfo KeyPropertyInfo { get; }

    // Constructors
    public KeyRepository(
        DbContext dbContext,
        Expression<Func<TEntity, TKey>> keyPropertyLambda
    )
        : base(dbContext)
    {
        KeyPropertyInfo = keyPropertyLambda.Body.GetPropertyInfo();
    }

    // Methods
    public async Task<bool> ExistsAsync(TKey key, CancellationToken cancellationToken = default)
        => await DbSet.AnyAsync(KeyEquals(key), cancellationToken);

    public async Task<TEntity?> GetAsync(TKey key, CancellationToken cancellationToken = default)
        => await DbSet.FindAsync(keyValues: new object?[] { key }, cancellationToken);


    public async Task<ImmutableArray<TEntity>> GetManyAsync(TKey key, params TKey[] additionalKeys)
        => await GetManyAsync(additionalKeys.Prepend(key));

    public async Task<ImmutableArray<TEntity>> GetManyAsync(IEnumerable<TKey> keys, CancellationToken cancellationToken = default)
    {
        if (!keys.Any())
        {
            return ImmutableArray<TEntity>.Empty;
        }

        ParameterExpression parameterExpression = Expression.Parameter(typeof(TEntity), "entity");
        MethodCallExpression keysContainMethodExpression;
        if (keys is ImmutableArray<TKey>)
        {
            MethodInfo containsMethodInfo = typeof(ImmutableArray<TKey>)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Single(method
                    => method.Name == nameof(ImmutableArray<TKey>.Contains)
                    && method.GetParameters().Length == 1
                );
            keysContainMethodExpression = Expression.Call(
                instance: Expression.Constant(keys),
                method: containsMethodInfo,
                Expression.Property(parameterExpression, KeyPropertyInfo)
            );
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
            keysContainMethodExpression = Expression.Call(
                instance: null,
                method: containsMethodInfo,
                Expression.Constant(keys), Expression.Property(parameterExpression, KeyPropertyInfo)
            );
        }
        Expression<Func<TEntity, bool>> keysContainEntityKeyLambda = Expression.Lambda<Func<TEntity, bool>>(
            keysContainMethodExpression,
            parameterExpression
        );
        ImmutableArray<TEntity> entities = await DbSet
            .Where(keysContainEntityKeyLambda)
            .ToImmutableArrayAsync(cancellationToken);
        return entities;
    }

    public async Task<ImmutableArray<TKey>> GetExistingIdsAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default)
        => await DbSet
            .Select(Key())
            .Where(id => ids.Contains(id))
            .ToImmutableArrayAsync(cancellationToken);

    public void Delete(TKey key)
    {
        TEntity mockEntity = Mock(key);
        DbSet.Remove(mockEntity);
    }


    public void DeleteMany(params TKey[] keys)
        => DeleteMany((IEnumerable<TKey>)keys);

    public void DeleteMany(IEnumerable<TKey> keys)
    {
        IEnumerable<TEntity> mockEntities = keys.Select(key => Mock(key));
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
    protected Expression<Func<TEntity, TKey>> Key()
    {
        ParameterExpression parameterExpression = Expression.Parameter(typeof(TEntity), "entity");
        Expression<Func<TEntity, TKey>> keyLambda = Expression.Lambda<Func<TEntity, TKey>>(
            Expression.Property(parameterExpression, KeyPropertyInfo),
            parameterExpression
        );
        return keyLambda;
    }

    protected Expression<Func<TEntity, bool>> KeyEquals(TKey key)
    {
        ParameterExpression parameterExpression = Expression.Parameter(typeof(TEntity), "entity");
        BinaryExpression keyPropertyEqualsKeyExpression = Expression.Equal(
            Expression.Property(parameterExpression, KeyPropertyInfo),
            Expression.Constant(key)
        );
        Expression<Func<TEntity, bool>> entityKeyFilterLambda = Expression.Lambda<Func<TEntity, bool>>(
            keyPropertyEqualsKeyExpression,
            parameterExpression
        );
        return entityKeyFilterLambda;
    }

    protected TEntity Mock(TKey key)
    {
        TEntity entity = (TEntity)Activator.CreateInstance(typeof(TEntity), nonPublic: true)!;
        KeyPropertyInfo.SetValue(entity, key);
        DbSet.Attach(entity);
        return entity;
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
