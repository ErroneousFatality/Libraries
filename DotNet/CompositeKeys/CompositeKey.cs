using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;

using AndrejKrizan.DotNet.Collections;
using AndrejKrizan.DotNet.Lambdas.Properties;

namespace AndrejKrizan.DotNet.CompositeKeys;
public abstract class CompositeKey<TEntity, TSelf>
    where TEntity : notnull
    where TSelf : CompositeKey<TEntity, TSelf>, ICompositeKey<TEntity, TSelf>
{
    // Static fields

    public static readonly ImmutableArray<PropertyBinding> PropertyBindings;

    // Static constructor

    static CompositeKey()
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

    public static string[] GetEntityPropertyNames()
        => PropertyBindings
            .Select(binding => binding.EntityProperty.PropertyInfo.Name)
            .ToArray();


    // Methods

    public object?[] GetValues()
        => PropertyBindings
            .Select(binding => binding.KeyProperty.GetValue(this))
            .ToArray();

    public Expression<Func<TEntity, bool>> ToEntityHasKeyPredicate()
        => Expression.Lambda<Func<TEntity, bool>>(
            PropertyBindings
                .Select(binding => binding.ToEntityPropertyEqualsKeyValueExpression(this))
                .Aggregate(Expression.AndAlso),
            PropertyBindings[0].EntityProperty.Parameter
        );

    public void InjectValuesInto(TEntity entity)
    {
        foreach (PropertyBinding propertyBinding in PropertyBindings)
        {
            object? value = propertyBinding.KeyProperty.PropertyInfo.GetValue(this);
            propertyBinding.EntityProperty.SetValue(entity, value);
        }
    }
}