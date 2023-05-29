using System.Linq.Expressions;

using AndrejKrizan.DotNet.ValueObjects.PropertyNavigations;

namespace AndrejKrizan.DotNet.ValueObjects.PropertyBindings;

public interface IPropertyBinding<TEntity, TKey>
    where TEntity : class
    where TKey : struct
{
    // Properties
    Type Type { get; }
    IPropertyNavigation<TEntity> EntityProperty { get; }
    IPropertyNavigation<TKey> KeyProperty { get; }

    // Methods
    Expression ToEqualsExpression(TKey key);
    MemberBinding ToKeyMemberBinding();
    void SetValue(TEntity entity, TKey key);
}
