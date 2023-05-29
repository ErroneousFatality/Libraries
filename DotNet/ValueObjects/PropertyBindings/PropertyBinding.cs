using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

using AndrejKrizan.DotNet.ValueObjects.PropertyNavigations;

namespace AndrejKrizan.DotNet.ValueObjects.PropertyBindings;

public readonly struct PropertyBinding<TEntity, TKey, TProperty> : IPropertyBinding<TEntity, TKey>
    where TEntity : class
    where TKey : struct
{
    // Properties
    public Type Type => EntityProperty.Type;
    public readonly required PropertyNavigation<TEntity, TProperty> EntityProperty { get; init; }
    public readonly required PropertyNavigation<TKey, TProperty> KeyProperty { get; init; }

    // Constructors

    [SetsRequiredMembers]
    public PropertyBinding(
        PropertyNavigation<TEntity, TProperty> entityProperty,
        PropertyNavigation<TKey, TProperty> keyProperty
    )
    {
        EntityProperty = entityProperty;
        KeyProperty = keyProperty;
    }

    [SetsRequiredMembers]
    public PropertyBinding(
        Expression<Func<TEntity, TProperty>> entityProperty,
        Expression<Func<TKey, TProperty>> keyProperty
    )
        : this(
            new PropertyNavigation<TEntity, TProperty>(entityProperty),
            new PropertyNavigation<TKey, TProperty>(keyProperty)
        )
    { }

    [SetsRequiredMembers]
    public PropertyBinding(
        ParameterExpression entityParameter, Expression entityProperty,
        ParameterExpression keyParameter, PropertyInfo keyProperty
    )
    : this(
        new PropertyNavigation<TEntity, TProperty>(entityParameter, entityProperty),
        new PropertyNavigation<TKey, TProperty>(keyParameter, Expression.Property(keyParameter, keyProperty))
    )
    { }

    public PropertyBinding() { }

    // Methods
    public readonly PropertyBinding<TEntity, TKey, TProperty> ReplaceParameters(ParameterExpression entity, ParameterExpression key)
        => new(EntityProperty.ReplaceParameter(entity), KeyProperty.ReplaceParameter(key));

    public readonly Expression ToEqualsExpression(TKey key)
        => EntityProperty.ToEqualsExpression(KeyProperty.GetValue(key));

    public readonly MemberBinding ToKeyMemberBinding()
        => Expression.Bind(KeyProperty.PropertyInfo, EntityProperty.Expression);

    public readonly void SetValue(TEntity entity, TKey key)
        => EntityProperty.SetValue(entity, KeyProperty.GetValue(key));


    // Interface implementations
    IPropertyNavigation<TEntity> IPropertyBinding<TEntity, TKey>.EntityProperty => EntityProperty;
    IPropertyNavigation<TKey> IPropertyBinding<TEntity, TKey>.KeyProperty => KeyProperty;
}
