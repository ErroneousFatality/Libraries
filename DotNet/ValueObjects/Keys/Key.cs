using System.Collections.Immutable;
using System.Linq.Expressions;

using AndrejKrizan.DotNet.Extensions;

namespace AndrejKrizan.DotNet.ValueObjects.Keys;

public abstract class Key<T, TSelf>
    where T : class
    where TSelf : Key<T, TSelf>
{
    // Properties
    public readonly ImmutableArray<IKeyPropertyBinding<T, TSelf>> PropertyBindings;
    public readonly ParameterExpression ObjectParameter;
    public readonly ParameterExpression KeyParameter;

    // Constructors
    protected Key(
        IKeyPropertyBinding<T, TSelf> propertyBinding,
        params IKeyPropertyBinding<T, TSelf>[] additionalPropertyBindings
    )
    {
        ObjectParameter = Expression.Parameter(typeof(T), typeof(T).Name.ToLowercasedFirstCharacterInvariant());
        KeyParameter = Expression.Parameter(typeof(TSelf), "key");
        PropertyBindings = additionalPropertyBindings.Prepend(propertyBinding).Convert(binding => binding.ReplaceParameters(ObjectParameter, KeyParameter));
    }

    // Methods
    public object?[] ToValues()
        => PropertyBindings.Select(binding => binding.Key.GetValue((TSelf)this)).ToArray();

    public Expression<Func<T, bool>> ToPredicateLambda()
        => Expression.Lambda<Func<T, bool>>(
            PropertyBindings.Select(binding => binding.ToEqualsExpression((TSelf)this)).Aggregate(Expression.AndAlso),
            ObjectParameter
        );

    public Expression<Func<T, TSelf>> ToKeyLambda()
        => Expression.Lambda<Func<T, TSelf>>(
            Expression.MemberInit(
                Expression.New(typeof(TSelf).GetConstructor(Array.Empty<Type>())!),
                PropertyBindings.Select(binding => binding.ToKeyMemberBinding())
            ),
            ObjectParameter
        );

    public T ToObject()
    {
        T obj = (T)Activator.CreateInstance(typeof(T), nonPublic: true)!;
        foreach (IKeyPropertyBinding<T, TSelf> property in PropertyBindings)
        {
            property.SetValue(obj, (TSelf)this);
        }
        return obj;
    }
}
