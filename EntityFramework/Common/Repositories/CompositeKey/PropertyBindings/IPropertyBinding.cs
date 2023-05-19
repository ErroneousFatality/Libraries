using System.Linq.Expressions;
using AndrejKrizan.DotNet.ValueObjects.PropertyNavigations;

namespace AndrejKrizan.EntityFramework.Common.Repositories.CompositeKey.PropertyBindings;

public interface IPropertyBinding<TEntity, TKey>
    where TEntity : class
    where TKey : struct
{
    // Properties
    IPropertyNavigation<TEntity> EntityProperty { get; }
    IPropertyNavigation<TKey> KeyProperty { get; }

    // Methods
    Expression ToEqualsExpression(TKey key);
    MemberBinding ToKeyMemberBinding();
    void SetValue(TEntity entity, TKey key);
}
