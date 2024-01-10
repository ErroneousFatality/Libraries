using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;

using AndrejKrizan.DotNet.Collections;
using AndrejKrizan.DotNet.Lambdas.Properties;

namespace AndrejKrizan.DotNet.CompositeKeys;
public interface ICompositeKey<TEntity, TSelf>
    where TEntity : notnull
    where TSelf : ICompositeKey<TEntity, TSelf>
{
    // Static properties

    /// <summary>Must use an object initializer.</summary>
    /// <example><code>
    ///     (Entity entity) => new EntityKey {{ A = entity.A, B = entity.B}} 
    /// </code></example>
    static abstract Expression<Func<TEntity, TSelf>> Selector { get; }

    // Static fields
    private static readonly ImmutableArray<PropertyBinding> PropertyBindings;

    // Static constructor
    static ICompositeKey()
    {
        const string errorMessage = "The selector expression must use an object initializer. For example: entity => new EntityKey {{ A = entity.A, B = entity.B }}.";
        Expression<Func<TEntity, TSelf>> selector = TSelf.Selector;
        if (selector.Body is not MemberInitExpression initialization)
        {
            throw new Exception(errorMessage);
        }
        ParameterExpression entity = selector.Parameters[0];
        ParameterExpression key = Expression.Parameter(typeof(TSelf), "key");
        PropertyBindings = initialization.Bindings.Convert((MemberBinding memberBinding) =>
        {
            if (
                memberBinding is not MemberAssignment propertyAssignment ||
                propertyAssignment.Member is not PropertyInfo property
            )
            {
                throw new Exception(errorMessage);
            }
            PropertyLambda entityProperty = new(Expression.Lambda(propertyAssignment.Expression, entity));
            PropertyLambda keyProperty = new(Expression.Lambda(Expression.Property(key, property), key));
            PropertyBinding propertyBinding = new(entityProperty, keyProperty);
            return propertyBinding;
        });
        if (PropertyBindings.Length < 1)
            throw new Exception("The selector expression must have at least one property assignment.");
    }

    // Static methods
    static string[] GetEntityPropertyNames()
        => PropertyBindings.Select(binding => binding.EntityProperty.PropertyInfo.Name).ToArray();

    // Methods
    object?[] GetValues()
        => PropertyBindings.Select(binding => binding.KeyProperty.GetValue((TSelf)this)).ToArray();


    Expression<Func<TEntity, bool>> ToEntityPredicate()
        => Expression.Lambda<Func<TEntity, bool>>(
            PropertyBindings
                .Select(binding => binding.ToEntityPropertyEqualsKeyValueExpression((TSelf)this))
                .Aggregate(Expression.AndAlso),
            PropertyBindings[0].EntityProperty.Parameter
        );

    void InjectValuesInto(TEntity entity)
    {
        foreach (PropertyBinding propertyBinding in PropertyBindings)
        {
            object? value = propertyBinding.KeyProperty.PropertyInfo.GetValue((TSelf)this);
            propertyBinding.EntityProperty.SetValue(entity, value);
        }
    }
}
