using System.Linq.Expressions;

using AndrejKrizan.EntityFramework.Common.ValueObjects;

namespace AndrejKrizan.EntityFramework.Common.Repositories;

public interface IEntityKeyPropertyBinding<TEntity, TKey>
    where TEntity : class
    where TKey : EntityKey<TEntity, TKey>
{
    // Properties
    IPropertyNavigation<TEntity> Entity { get; }
    IPropertyNavigation<TKey> Key { get; }

    // Methods
    IEntityKeyPropertyBinding<TEntity, TKey> ReplaceParameters(ParameterExpression entity, ParameterExpression key);
    Expression ToEqualsExpression(TKey key);
    MemberBinding ToKeyMemberBinding();
    void SetValue(TEntity entity, TKey key);
}

public class EntityKeyPropertyBinding<TEntity, TKey, TProperty> : IEntityKeyPropertyBinding<TEntity, TKey>
    where TEntity : class
    where TKey : EntityKey<TEntity, TKey>
{
    // Properties
    public PropertyNavigation<TEntity, TProperty> Entity { get; private set; }
    public PropertyNavigation<TKey, TProperty> Key { get; }

    // Constructors
    public EntityKeyPropertyBinding(
        PropertyNavigation<TEntity, TProperty> entity,
        PropertyNavigation<TKey, TProperty> key
    )
    {
        Entity = entity;
        Key = key;
    }

    public EntityKeyPropertyBinding(
        Expression<Func<TEntity, TProperty>> entity,
        Expression<Func<TKey, TProperty>> key
    )
        : this(
            new PropertyNavigation<TEntity, TProperty>(entity),
            new PropertyNavigation<TKey, TProperty>(key)
        )
    { }

    // Methods
    public EntityKeyPropertyBinding<TEntity, TKey, TProperty> ReplaceParameters(ParameterExpression entity, ParameterExpression key)
        => new(Entity.ReplaceParameter(entity), Key.ReplaceParameter(key));

    public Expression ToEqualsExpression(TKey key)
        => Entity.ToEqualsExpression(Key.GetValue(key));

    public MemberBinding ToKeyMemberBinding()
        => Expression.Bind(Key.PropertyInfo, Entity.Expression);

    public void SetValue(TEntity entity, TKey key)
        => Entity.SetValue(entity, Key.GetValue(key));


    // Interface implementations
    IPropertyNavigation<TEntity> IEntityKeyPropertyBinding<TEntity, TKey>.Entity => Entity;
    IPropertyNavigation<TKey> IEntityKeyPropertyBinding<TEntity, TKey>.Key => Key;

    IEntityKeyPropertyBinding<TEntity, TKey> IEntityKeyPropertyBinding<TEntity, TKey>.ReplaceParameters(ParameterExpression entity, ParameterExpression key)
        => ReplaceParameters(entity, key);
}
