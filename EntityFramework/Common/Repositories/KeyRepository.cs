using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;

using AndrejKrizan.DotNet.Collections;
using AndrejKrizan.DotNet.Lambdas.Properties;
using AndrejKrizan.DotNet.Repositories;
using AndrejKrizan.DotNet.Utilities;
using AndrejKrizan.EntityFramework.Common.Queries;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

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


    public async Task<ImmutableArray<TEntity>> GetManyAsync<TCollection>(TCollection keys, CancellationToken cancellationToken = default)
        where TCollection: IReadOnlyCollection<TKey>
    {
        if (keys.Count == 0) { return []; }
        ImmutableArray<TEntity> entities = await DbSet
            .Where(KeyIsIn(keys))
            .ToImmutableArrayAsync(keys.Count, cancellationToken);
        return entities;
    }

    public async Task<ImmutableArray<TEntity>> GetManyAsync(IEnumerable<TKey> keys, CancellationToken cancellationToken = default)
        => keys is ImmutableArray<TKey> immutableArray ? await GetManyAsync(immutableArray, cancellationToken)
            : keys is IReadOnlyCollection<TKey> collection ? await GetManyAsync(collection, cancellationToken)
                : await GetManyAsync(keys.ToImmutableArray(), cancellationToken);


    public async Task<ImmutableArray<TKey>> GetKeysAsync<TCollection>(TCollection keys, CancellationToken cancellationToken = default)
        where TCollection: IReadOnlyCollection<TKey>
    {
        if (keys.Count == 0) { return []; }
        ImmutableArray<TKey> _keys = await DbSet
            .Select(KeyProperty.Lambda)
            .Where(key => keys.Contains(key))
            .ToImmutableArrayAsync(keys.Count, cancellationToken);
        return _keys;
    }

    public async Task<ImmutableArray<TKey>> GetKeysAsync(IEnumerable<TKey> keys, CancellationToken cancellationToken = default)
        => keys is ImmutableArray<TKey> immutableArray ? await GetKeysAsync(immutableArray, cancellationToken)
                : keys is IReadOnlyCollection<TKey> collection ? await GetKeysAsync(collection, cancellationToken)
                    : await GetKeysAsync(keys.ToImmutableArray(), cancellationToken);


    /// <returns>Whether the key was found and deleted.</returns>
    public async Task<bool> DeleteAsync(TKey key, CancellationToken cancellationToken = default)
    {
        int deletedCount = await DbSet
            .Where(KeyProperty.ToEqualsLambda<TEntity>(key))
            .ExecuteDeleteAsync(cancellationToken);
        return deletedCount > 0;
    }

    /// <returns>Whether the entity was found and deleted by its key.</returns>
    public async Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        TKey key = KeyProperty.GetValue(entity);
        int deletedCount = await DbSet
            .Where(KeyProperty.ToEqualsLambda<TEntity>(key))
            .ExecuteDeleteAsync(cancellationToken);
        return deletedCount > 0;
    }

    /// <returns>The number of keys found and deleted.</returns>
    public async Task<int> DeleteManyAsync(IEnumerable<TKey> keys, CancellationToken cancellationToken = default)
    {
        int deletedCount = await DbSet
            .Where(KeyIsIn(keys))
            .ExecuteDeleteAsync(cancellationToken);
        return deletedCount;
    }

    /// <returns>The number of keys found and deleted.</returns>
    public async Task<int> DeleteManyAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        ImmutableArray<TKey> keys = entities.Convert(KeyProperty.GetValue);
        int deletedCount = await DbSet
            .Where(KeyIsIn(keys))
            .ExecuteDeleteAsync(cancellationToken);
        return deletedCount;
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
    public Task<bool> TryDeleteAsync(TKey key, CancellationToken cancellationToken = default)
        => DeleteAsync(key, cancellationToken);

    [Obsolete(message: $"Use the {nameof(DeleteMany)} with an inline initialized collection instead. E.g.: [1, 2, 3]", error: true)]
    public void DeleteMany(params TKey[] keys)
        => DeleteMany((IEnumerable<TKey>)keys);

    // Protected methods
    protected virtual TEntity MockEntity(TKey key)
    {
        TEntity entity = Utils.CreateDefaultInstance<TEntity>();
        KeyProperty.SetValue(entity, key);
        DbSet.Attach(entity);
        return entity;
    }

    protected Expression<Func<TEntity, bool>> KeyIsIn<TKeyCollection>(TKeyCollection keys)
        where TKeyCollection: IEnumerable<TKey>
    {
        MethodCallExpression keysContainKey;
        if (keys is ImmutableArray<TKey> immutableKeys)
        {
            MethodInfo contains = typeof(ImmutableArray<TKey>)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Single(method
                    => method.Name == nameof(ImmutableArray<>.Contains)
                    && method.GetParameters().Length == 1
                );
            keysContainKey = Expression.Call(
                instance: Expression.Constant(immutableKeys),
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
            keysContainKey = Expression.Call(
                instance: null,
                method: contains,
                arguments: [Expression.Constant(keys), KeyProperty.Expression]
            );
        }
        Expression<Func<TEntity, bool>> keyIsInKeys = Expression.Lambda<Func<TEntity, bool>>(keysContainKey, KeyProperty.Parameter);
        return keyIsInKeys;
    }
}
