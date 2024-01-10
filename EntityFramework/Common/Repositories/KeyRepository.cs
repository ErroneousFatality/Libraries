using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;

using AndrejKrizan.DotNet.Lambdas.Properties;
using AndrejKrizan.DotNet.Utilities;
using AndrejKrizan.EntityFramework.Common.Queries;

using Microsoft.EntityFrameworkCore;

namespace AndrejKrizan.EntityFramework.Common.Repositories;

public abstract class KeyRepository<TEntity, TKey> : Repository<TEntity>, IKeyRepository<TEntity, TKey>
    where TEntity : class
{
    // Protected properties
    protected abstract Expression<Func<TEntity, TKey>> KeySelector { get; }

    protected PropertyLambda<TEntity, TKey> KeyProperty => _keyProperty ??= new PropertyLambda<TEntity, TKey>(KeySelector);
    private static PropertyLambda<TEntity, TKey>? _keyProperty;

    // Constructors
    public KeyRepository(DbContext dbContext)
        : base(dbContext) { }

    // Methods

    public async Task<bool> ExistsAsync(TKey key, CancellationToken cancellationToken = default)
        => await DbSet.AnyAsync(KeyProperty.ToEqualsLambda<TEntity>(key), cancellationToken);


    public async Task<TEntity?> GetAsync(TKey key, CancellationToken cancellationToken = default)
        => await DbSet.FindAsync([key], cancellationToken);

    public async Task<ImmutableArray<TEntity>> GetManyAsync(IEnumerable<TKey> keys, CancellationToken cancellationToken = default)
    {
        if (!keys.Any())
        {
            return [];
        }

        MethodCallExpression _keysContainKey;
        if (keys is ImmutableArray<TKey> _keys)
        {
            MethodInfo contains = typeof(ImmutableArray<TKey>)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Single(method
                    => method.Name == nameof(ImmutableArray<TKey>.Contains)
                    && method.GetParameters().Length == 1
                );
            _keysContainKey = Expression.Call(
                instance: Expression.Constant(_keys),
                method: contains,
                arguments: [KeyProperty.Expression]
            );
        }
        else
        {
            MethodInfo contains = typeof(Enumerable)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Single(method
                    => method.Name == nameof(Enumerable.Contains)
                    && method.GetParameters().Length == 2
                )
                .MakeGenericMethod(typeof(TKey));
            _keysContainKey = Expression.Call(
                instance: null,
                method: contains,
                arguments: [Expression.Constant(keys), KeyProperty.Expression]
            );
        }
        Expression<Func<TEntity, bool>> keysContainKey = Expression.Lambda<Func<TEntity, bool>>(_keysContainKey, KeyProperty.Parameter);

        ImmutableArray<TEntity> entities = await DbSet
            .Where(keysContainKey)
            .ToImmutableArrayAsync(cancellationToken);
        return entities;
    }

    public async Task<ImmutableArray<TKey>> GetKeysAsync(IEnumerable<TKey> keys, CancellationToken cancellationToken = default)
        => await DbSet
            .Select(KeyProperty.Lambda)
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

    // Protected methods
    protected virtual TEntity MockEntity(TKey key)
    {
        TEntity entity = Utils.CreateDefaultInstance<TEntity>();
        KeyProperty.SetValue(entity, key);
        DbSet.Attach(entity);
        return entity;
    }
}
