using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;

using AndrejKrizan.DotNet.Entities;
using AndrejKrizan.DotNet.Extensions;
using AndrejKrizan.DotNet.Utilities;
using AndrejKrizan.DotNet.ValueObjects.PropertyBindings;
using AndrejKrizan.EntityFramework.Common.Extensions.IQueryables;

using Microsoft.EntityFrameworkCore;

namespace AndrejKrizan.EntityFramework.Common.Repositories;

public abstract class CompositeKeyRepository<TEntity, TKey> : Repository<TEntity>, IKeyRepository<TEntity, TKey>
    where TEntity : class
    where TKey : struct, IKey<TEntity>
{
    // Constructors
    public CompositeKeyRepository(DbContext dbContext)
        : base(dbContext) { }

    // Methods

    public async Task<bool> ExistsAsync(TKey key, CancellationToken cancellationToken = default)
        => await DbSet.AnyAsync(CreateKeyPredicateLambda(key), cancellationToken);

    public async Task<TEntity?> GetAsync(TKey key, CancellationToken cancellationToken = default)
    {
        object?[] keyValues = KeyPropertyBindings.Select(binding => binding.KeyProperty.GetValue(key)).ToArray();
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
            .Select(KeyLambda)
            .ToImmutableArrayAsync(cancellationToken);

    public async Task<ImmutableArray<TKey>> GetKeysAsync(IEnumerable<TKey> keys, int chunkSize, CancellationToken cancellationToken = default)
        => await DbSet.WhereAnyAsync(keys, chunkSize,
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

    // Protected properties
    protected static readonly Expression<Func<TEntity, TKey>> KeyLambda;
    protected static readonly ParameterExpression EntityParameter;
    protected static readonly ImmutableArray<IPropertyBinding<TEntity, TKey>> KeyPropertyBindings;

    static CompositeKeyRepository()
    {
        KeyLambda = TKey.Lambda.UnwrapConversionsUntilType<TEntity, object?, TKey>();
        EntityParameter = KeyLambda.Parameters.Single();

        const string errorMessage = $"The {nameof(KeyLambda)} expression must use an object initializer. For example: entity => new EntityKey {{ A = entity.A, B = entity.B}}";
        if (KeyLambda.Body is not MemberInitExpression initialization)
        {
            throw new Exception(errorMessage);
        }
        ParameterExpression keyParameter = Expression.Parameter(typeof(TKey), "key");
        KeyPropertyBindings = initialization.Bindings.Convert((MemberBinding memberBinding) =>
        {
            if (memberBinding.Member is not PropertyInfo keyProperty || memberBinding is not MemberAssignment keyAssignment)
            {
                throw new Exception(errorMessage);
            }
            Expression entityProperty = keyAssignment.Expression;
            Type bindingType = typeof(PropertyBinding<,,>).MakeGenericType(typeof(TEntity), typeof(TKey), keyProperty.PropertyType);
            IPropertyBinding<TEntity, TKey> propertyBinding = (IPropertyBinding<TEntity, TKey>)Activator.CreateInstance(bindingType, EntityParameter, entityProperty, keyParameter, keyProperty)!;
            return propertyBinding;
        });
    }

    // Protected methods
    protected Expression<Func<TEntity, bool>> CreateKeyPredicateLambda(TKey key)
        => Expression.Lambda<Func<TEntity, bool>>(
            KeyPropertyBindings
                .Select(binding => binding.ToEqualsExpression(key))
                .Aggregate(Expression.AndAlso),
            EntityParameter
        );

    protected TEntity MockEntity(TKey key)
    {
        TEntity entity = Utils.CreateDefaultInstance<TEntity>();
        foreach (IPropertyBinding<TEntity, TKey> keyProperty in KeyPropertyBindings)
        {
            keyProperty.SetValue(entity, key);
        }
        DbSet.Attach(entity);
        return entity;
    }
}
