using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;
using AndrejKrizan.DotNet.Entities;
using AndrejKrizan.DotNet.Extensions;
using AndrejKrizan.DotNet.Utilities;
using AndrejKrizan.DotNet.ValueObjects.PropertyBindings;
using AndrejKrizan.DotNet.ValueObjects.PropertyNavigations;
using AndrejKrizan.EntityFramework.Common.Extensions.IQueryables;

using Microsoft.EntityFrameworkCore;

namespace AndrejKrizan.EntityFramework.Common.Repositories;

public class CompositeKeyRepository<TEntity, TKey> : Repository<TEntity>
    where TEntity : Entity<TKey, TEntity>
    where TKey : struct
{
    // Constructors
    public CompositeKeyRepository(DbContext dbContext)
        : base(dbContext) { }

    // Methods

    public async Task<bool> ExistsAsync(TKey key, CancellationToken cancellationToken = default)
        => await DbSet.AnyAsync(CreateKeyPredicateLambda(key), cancellationToken);

    public async Task<TEntity?> GetAsync(TKey key, CancellationToken cancellationToken = default)
    {
        object?[] keyValues = IsSimpleKey
            ? new object?[] { key }
            : KeyPropertyBindings.Select(binding => binding.KeyProperty.GetValue(key)).ToArray();
        TEntity? entity = await DbSet.FindAsync(keyValues, cancellationToken);
        return entity;
    }


    public async Task<ImmutableArray<TEntity>> GetManyAsync(IEnumerable<TKey> keys, CancellationToken cancellationToken = default)
        => IsSimpleKey
        ? await SimpleKeyContainedInAsync(keys, cancellationToken)
        : await DbSet
            .WhereAny(keys, CreateKeyPredicateLambda)
            .ToImmutableArrayAsync(cancellationToken);

    public async Task<ImmutableArray<TEntity>> GetManyAsync(IEnumerable<TKey> keys, int chunkSize, CancellationToken cancellationToken = default)
        => IsSimpleKey
        ? await SimpleKeyContainedInAsync(keys, cancellationToken)
        : await DbSet.WhereAnyAsync(keys, chunkSize, CreateKeyPredicateLambda, cancellationToken);


    public async Task<ImmutableArray<TKey>> GetKeysAsync(IEnumerable<TKey> keys, CancellationToken cancellationToken = default)
        => IsSimpleKey
        ? await GetSimpleKeysAsync(keys, cancellationToken)
        : await DbSet
            .WhereAny(keys, CreateKeyPredicateLambda)
            .Select(KeyLambda)
            .ToImmutableArrayAsync(cancellationToken);

    public async Task<ImmutableArray<TKey>> GetKeysAsync(IEnumerable<TKey> keys, int chunkSize, CancellationToken cancellationToken = default)
    => IsSimpleKey
        ? await GetSimpleKeysAsync(keys, cancellationToken)
        : await DbSet.WhereAnyAsync(keys, chunkSize,
            CreateKeyPredicateLambda,
            query => query.Select(KeyLambda),
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
    protected virtual Expression<Func<TEntity, bool>> CreateKeyPredicateLambda(TKey key)
        => IsSimpleKey
        ? KeyNavigation!.ToEqualsLambda(key)
        : Expression.Lambda<Func<TEntity, bool>>(
            KeyPropertyBindings
                .Select(binding => binding.ToEqualsExpression(key))
                .Aggregate(Expression.AndAlso),
            EntityParameter
        );

    protected virtual TEntity MockEntity(TKey key)
    {
        TEntity entity = Utils.CreateDefaultInstance<TEntity>();
        if (IsSimpleKey)
        {
            KeyNavigation!.SetValue(entity, key);
        }
        else foreach (IPropertyBinding<TEntity, TKey> keyProperty in KeyPropertyBindings)
        {
            keyProperty.SetValue(entity, key);
        }
        DbSet.Attach(entity);
        return entity;
    }

    protected virtual async Task<ImmutableArray<TEntity>> SimpleKeyContainedInAsync(IEnumerable<TKey> keys, CancellationToken cancellationToken = default)
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
            keysContain = Expression.Call(instance: Expression.Constant(keys), method: containsMethodInfo, KeyNavigation!.Expression);
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
            keysContain = Expression.Call(instance: null, method: containsMethodInfo, KeyNavigation!.Expression);
        }
        Expression<Func<TEntity, bool>> keysContainEntityKeyLambda = Expression.Lambda<Func<TEntity, bool>>(keysContain, EntityParameter);
        ImmutableArray<TEntity> entities = await DbSet
            .Where(keysContainEntityKeyLambda)
            .ToImmutableArrayAsync(cancellationToken);
        return entities;
    }
    protected virtual async Task<ImmutableArray<TKey>> GetSimpleKeysAsync(IEnumerable<TKey> keys, CancellationToken cancellationToken = default)
        => await DbSet
            .Select(KeyLambda)
            .Where(key => keys.Contains(key))
            .ToImmutableArrayAsync(cancellationToken);


    // Static
    protected static readonly Expression<Func<TEntity, TKey>> KeyLambda;
    protected static readonly ParameterExpression EntityParameter;

    protected static readonly PropertyNavigation<TEntity, TKey>? KeyNavigation;
    protected static readonly ImmutableArray<IPropertyBinding<TEntity, TKey>> KeyPropertyBindings;

    public static readonly bool IsSimpleKey;

    static CompositeKeyRepository()
    {
        KeyLambda = Utils.GetFromDefaultInstance((TEntity entity) => entity.KeyLambda);
        EntityParameter = KeyLambda.Parameters.Single();

        if (KeyLambda.Body is MemberInitExpression initialization)
        {
            ParameterExpression keyParameter = Expression.Parameter(typeof(TKey), "key");
            KeyPropertyBindings = initialization.Bindings
                .Select((memberBinding) =>
                {
                    if (memberBinding.Member is not PropertyInfo keyProperty || memberBinding is not MemberAssignment keyAssignment)
                    {
                        throw new Exception($"The {nameof(KeyLambda)} expression must use an object initializer. For example: entity => new EntityKey {{ A = entity.A, B = entity.B}}");
                    }
                    Expression entityProperty = keyAssignment.Expression;
                    Type bindingType = typeof(PropertyBinding<,,>).MakeGenericType(typeof(TEntity), typeof(TKey), keyProperty.PropertyType);
                    IPropertyBinding<TEntity, TKey> propertyBinding = (IPropertyBinding<TEntity, TKey>)Activator.CreateInstance(bindingType, EntityParameter, entityProperty, keyParameter, keyProperty)!;
                    return propertyBinding;
                })
                .ToImmutableArray();
            IsSimpleKey = false;
        }
        else
        {
            KeyNavigation = new(KeyLambda);
            IsSimpleKey = true;
        }


    }
}
