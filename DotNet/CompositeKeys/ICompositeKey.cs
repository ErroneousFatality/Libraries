using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;

using AndrejKrizan.DotNet.Collections;
using AndrejKrizan.DotNet.CompositeKeys.PropertyBindings;

namespace AndrejKrizan.DotNet.CompositeKeys;
public interface ICompositeKey<TEntity, TSelf>
    where TEntity: class
    where TSelf : ICompositeKey<TEntity, TSelf>
{
    // Static properties

    /// <summary>Must use an object initializer.</summary>
    /// <example><code>
    ///     (Entity entity) => new EntityKey {{ A = entity.A, B = entity.B}} 
    /// </code></example>
    static abstract Expression<Func<TEntity, TSelf>> Selector { get; }

    // Static fields
    static readonly ParameterExpression EntityParameter;
    static readonly ParameterExpression KeyParameter;
    static readonly ImmutableArray<IKeyPropertyBinding<TEntity, TSelf>> PropertyBindings;

    // Static constructor
    static ICompositeKey()
    {
        const string errorMessage = $"The {nameof(TSelf.Selector)} expression must use an object initializer. For example: entity => new EntityKey {{ A = entity.A, B = entity.B}}";
        if (TSelf.Selector.Body is not MemberInitExpression initialization)
        {
            throw new Exception(errorMessage);
        }
        EntityParameter = TSelf.Selector.Parameters[0];
        KeyParameter = Expression.Parameter(typeof(TSelf), "key");
        PropertyBindings = initialization.Bindings.Convert((MemberBinding memberBinding) =>
        {
            if (memberBinding is not MemberAssignment assignment || assignment.Member is not PropertyInfo keyPropertyInfo)
            {
                throw new Exception(errorMessage);
            }
            return CreateKeyPropertyBinding(assignment.Expression, keyPropertyInfo);
        });
    }

    // Methods
    public Expression<Func<TEntity, bool>> ToEntityHasKeyLambda()
        => Expression.Lambda<Func<TEntity, bool>>(
            PropertyBindings
                .Select(binding => binding.ToEqualsExpression((TSelf)this))
                .Aggregate(Expression.AndAlso),
            EntityParameter
        );

    // Private static methods
    /// <remarks>
    /// Calls the <see cref="KeyPropertyBinding{TEntity, TKey, TProperty}.KeyPropertyBinding(ParameterExpression, Expression, ParameterExpression, PropertyInfo)"/> constructor using reflection.
    /// </remarks>
    private static IKeyPropertyBinding<TEntity, TSelf> CreateKeyPropertyBinding(Expression entityPropertyExpression, PropertyInfo keyPropertyInfo)
    {
        Type bindingType = typeof(KeyPropertyBinding<,,>).MakeGenericType(typeof(TEntity), typeof(TSelf), keyPropertyInfo.PropertyType);
        IKeyPropertyBinding<TEntity, TSelf> propertyBinding = (IKeyPropertyBinding<TEntity, TSelf>)Activator.CreateInstance(bindingType,
            EntityParameter, entityPropertyExpression,
            KeyParameter, keyPropertyInfo
        )!;
        return propertyBinding;
    }
}
