using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;

using AndrejKrizan.DotNet.CompositeKeys.KeyPropertyBindings;
using AndrejKrizan.DotNet.Extensions;

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

    // Private static fields
    static ParameterExpression EntityParameter => TSelf.Selector.Parameters[0];
    static readonly ImmutableArray<IKeyPropertyBinding<TEntity, TSelf>> PropertyBindings;

    // Static constructor
    static ICompositeKey()
    {
        const string errorMessage = $"The {nameof(TSelf.Selector)} expression must use an object initializer. For example: entity => new EntityKey {{ A = entity.A, B = entity.B}}";
        if (TSelf.Selector.Body is not MemberInitExpression initialization)
        {
            throw new Exception(errorMessage);
        }
        ParameterExpression keyParameter = Expression.Parameter(typeof(TSelf), "key");
        PropertyBindings = initialization.Bindings.Convert((memberBinding) =>
        {
            if (memberBinding.Member is not PropertyInfo keyProperty || memberBinding is not MemberAssignment keyAssignment)
            {
                throw new Exception(errorMessage);
            }
            Expression entityProperty = keyAssignment.Expression;
            Type bindingType = typeof(KeyPropertyBinding<,,>).MakeGenericType(typeof(TEntity), typeof(TSelf), keyProperty.PropertyType);
            IKeyPropertyBinding<TEntity, TSelf> propertyBinding = (IKeyPropertyBinding<TEntity, TSelf>)Activator.CreateInstance(bindingType, EntityParameter, entityProperty, keyParameter, keyProperty)!;
            return propertyBinding;
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


}
