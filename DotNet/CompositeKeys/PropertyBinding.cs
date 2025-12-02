using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

using AndrejKrizan.DotNet.Lambdas.Properties;

namespace AndrejKrizan.DotNet.CompositeKeys;
public readonly struct PropertyBinding
{
    // Properties
    public readonly required PropertyLambda EntityProperty { get; init; }
    public readonly required PropertyLambda KeyProperty { get; init; }

    // Constructors
    public PropertyBinding() { }

    [SetsRequiredMembers]
    public PropertyBinding(PropertyLambda entityProperty, PropertyLambda keyProperty)
    {
        EntityProperty = entityProperty;
        KeyProperty = keyProperty;
    }

    // Methods

    public readonly BinaryExpression ToEntityPropertyEqualsKeyValueExpression(object key)
        => CreateEqualsExpression(EntityProperty, key, KeyProperty);

    public readonly LambdaExpression ToEntityPropertyEqualsKeyValueLambda(object key)
        => CreateEqualsLambda(EntityProperty, key, KeyProperty);


    public readonly BinaryExpression ToKeyPropertyEqualsEntityValueExpression(object entity)
        => CreateEqualsExpression(KeyProperty, entity, EntityProperty);

    public readonly LambdaExpression ToKeyPropertyEqualsEntityValueLambda(object entity)
        => CreateEqualsLambda(KeyProperty, entity, EntityProperty);


    public readonly PropertyBinding ReplaceParameters(ParameterExpression entity, ParameterExpression key)
        => new()
        {
            EntityProperty = EntityProperty.ReplaceParameter(entity),
            KeyProperty = KeyProperty.ReplaceParameter(key)
        };

    // Private methods
    private static BinaryExpression CreateEqualsExpression(PropertyLambda leftPropertyLambda, object rightSource, PropertyLambda rightPropertyLambda)
    {
        object? value = rightPropertyLambda.GetValue(rightSource);
        BinaryExpression expression = leftPropertyLambda.ToEqualsExpression(value);
        return expression;
    }

    private static LambdaExpression CreateEqualsLambda(PropertyLambda leftPropertyLambda, object rightSource, PropertyLambda rightPropertyLambda)
    {
        BinaryExpression expression = CreateEqualsExpression(leftPropertyLambda, rightSource, rightPropertyLambda);
        LambdaExpression lambda = Expression.Lambda(expression, leftPropertyLambda.Parameter);
        return lambda;
    }
}
