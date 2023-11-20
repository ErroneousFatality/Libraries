using System.Linq.Expressions;

using AndrejKrizan.DotNet.PropertyNavigations;

namespace AndrejKrizan.DotNet.CompositeKeys.PropertyBindings;

public interface IKeyPropertyBinding<TEntity, TKey>
    where TEntity : class
{
    // Properties
    IPropertyNavigation<TEntity> EntityNavigation { get; }
    IPropertyNavigation<TKey> KeyNavigation { get; }

    // Methods
    Expression ToEqualsExpression(TKey key);
    MemberBinding ToKeyMemberBinding();
    void SetValue(TEntity entity, TKey key);
}
