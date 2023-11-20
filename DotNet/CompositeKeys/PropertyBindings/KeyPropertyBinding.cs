using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

using AndrejKrizan.DotNet.PropertyNavigations;

namespace AndrejKrizan.DotNet.CompositeKeys.PropertyBindings;

public readonly struct KeyPropertyBinding<TEntity, TKey, TProperty> : IKeyPropertyBinding<TEntity, TKey>
    where TEntity : class
{
    // Properties
    public readonly required PropertyNavigation<TEntity, TProperty> EntityNavigation { get; init; }
    public readonly required PropertyNavigation<TKey, TProperty> KeyNavigation { get; init; }

    // Constructors

    [SetsRequiredMembers]
    public KeyPropertyBinding(
        PropertyNavigation<TEntity, TProperty> entityNavigation,
        PropertyNavigation<TKey, TProperty> keyNavigation
    )
    {
        EntityNavigation = entityNavigation;
        KeyNavigation = keyNavigation;
    }

    [SetsRequiredMembers]
    public KeyPropertyBinding(
        Expression<Func<TEntity, TProperty>> entityProperty,
        Expression<Func<TKey, TProperty>> keyProperty
    )
        : this(
            new PropertyNavigation<TEntity, TProperty>(entityProperty),
            new PropertyNavigation<TKey, TProperty>(keyProperty)
        )
    { }

    [SetsRequiredMembers]
    public KeyPropertyBinding(
        ParameterExpression entityParameter, Expression entityProperty,
        ParameterExpression keyParameter, PropertyInfo keyProperty
    )
    : this(
        new PropertyNavigation<TEntity, TProperty>(entityParameter, entityProperty),
        new PropertyNavigation<TKey, TProperty>(keyParameter, Expression.Property(keyParameter, keyProperty))
    )
    { }

    public KeyPropertyBinding() { }

    // Methods
    public readonly KeyPropertyBinding<TEntity, TKey, TProperty> ReplaceParameters(ParameterExpression entity, ParameterExpression key)
        => new(EntityNavigation.ReplaceParameter(entity), KeyNavigation.ReplaceParameter(key));

    public readonly Expression ToEqualsExpression(TKey key)
        => EntityNavigation.CreateEqualsExpression(KeyNavigation.GetValue(key));

    public readonly MemberBinding ToKeyMemberBinding()
        => Expression.Bind(KeyNavigation.Info, EntityNavigation.Expression);

    public readonly void SetValue(TEntity entity, TKey key)
        => EntityNavigation.SetValue(entity, KeyNavigation.GetValue(key));


    // Interface implementations
    IPropertyNavigation<TEntity> IKeyPropertyBinding<TEntity, TKey>.EntityNavigation => EntityNavigation;
    IPropertyNavigation<TKey> IKeyPropertyBinding<TEntity, TKey>.KeyNavigation => KeyNavigation;
}
