using System.Linq.Expressions;

namespace AndrejKrizan.DotNet.ValueObjects.Keys;

public interface IKeyPropertyBinding<T, TKey>
    where T : class
    where TKey : Key<T, TKey>
{
    // Properties
    IPropertyNavigation<T> Object { get; }
    IPropertyNavigation<TKey> Key { get; }

    // Methods
    IKeyPropertyBinding<T, TKey> ReplaceParameters(ParameterExpression obj, ParameterExpression key);
    Expression ToEqualsExpression(TKey key);
    MemberBinding ToKeyMemberBinding();
    void SetValue(T obj, TKey key);
}

public class KeyPropertyBinding<T, TKey, TProperty> : IKeyPropertyBinding<T, TKey>
    where T : class
    where TKey : Key<T, TKey>
{
    // Properties
    public PropertyNavigation<T, TProperty> Object { get; private set; }
    public PropertyNavigation<TKey, TProperty> Key { get; }

    // Constructors
    public KeyPropertyBinding(
        PropertyNavigation<T, TProperty> obj,
        PropertyNavigation<TKey, TProperty> key
    )
    {
        Object = obj;
        Key = key;
    }

    public KeyPropertyBinding(
        Expression<Func<T, TProperty>> obj,
        Expression<Func<TKey, TProperty>> key
    )
        : this(
            new PropertyNavigation<T, TProperty>(obj),
            new PropertyNavigation<TKey, TProperty>(key)
        )
    { }

    // Methods
    public KeyPropertyBinding<T, TKey, TProperty> ReplaceParameters(ParameterExpression obj, ParameterExpression key)
        => new(Object.ReplaceParameter(obj), Key.ReplaceParameter(key));

    public Expression ToEqualsExpression(TKey key)
        => Object.ToEqualsExpression(Key.GetValue(key));

    public MemberBinding ToKeyMemberBinding()
        => Expression.Bind(Key.PropertyInfo, Object.Expression);

    public void SetValue(T obj, TKey key)
        => Object.SetValue(obj, Key.GetValue(key));


    // Interface implementations
    IPropertyNavigation<T> IKeyPropertyBinding<T, TKey>.Object => Object;
    IPropertyNavigation<TKey> IKeyPropertyBinding<T, TKey>.Key => Key;

    IKeyPropertyBinding<T, TKey> IKeyPropertyBinding<T, TKey>.ReplaceParameters(ParameterExpression obj, ParameterExpression key)
        => ReplaceParameters(obj, key);
}
