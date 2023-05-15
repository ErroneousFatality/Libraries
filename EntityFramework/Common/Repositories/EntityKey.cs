using System.Collections.Immutable;
using System.Linq.Expressions;

using AndrejKrizan.DotNet.Extensions;

namespace AndrejKrizan.EntityFramework.Common.Repositories;

public abstract class EntityKey<TEntity, TSelf>
    where TEntity : class
    where TSelf : EntityKey<TEntity, TSelf>
{
    // Properties
    public readonly ImmutableArray<IEntityKeyPropertyBinding<TEntity, TSelf>> PropertyBindings;
    public readonly ParameterExpression EntityParameter;
    public readonly ParameterExpression KeyParameter;

    // Constructors
    protected EntityKey(
        IEntityKeyPropertyBinding<TEntity, TSelf> propertyBinding,
        params IEntityKeyPropertyBinding<TEntity, TSelf>[] additionalPropertyBindings
    )
    {
        EntityParameter = Expression.Parameter(typeof(TEntity), typeof(TEntity).Name.ToLowercasedFirstCharacterInvariant());
        KeyParameter = Expression.Parameter(typeof(TSelf), "key");
        PropertyBindings = additionalPropertyBindings.Prepend(propertyBinding).Convert(binding => binding.ReplaceParameters(EntityParameter, KeyParameter));
    }

    // Methods
    public object?[] ToValues()
        => PropertyBindings.Select(binding => binding.Key.GetValue((TSelf)this)).ToArray();

    public Expression<Func<TEntity, bool>> ToPredicateLambda()
        => Expression.Lambda<Func<TEntity, bool>>(
            PropertyBindings.Select(binding => binding.ToEqualsExpression((TSelf)this)).Aggregate(Expression.AndAlso),
            EntityParameter
        );

    public Expression<Func<TEntity, TSelf>> ToEntityToKeyLambda()
        => Expression.Lambda<Func<TEntity, TSelf>>(
            Expression.MemberInit(
                Expression.New(typeof(TSelf).GetConstructor(Array.Empty<Type>())!),
                PropertyBindings.Select(binding => binding.ToKeyMemberBinding())
            ),
            EntityParameter
        );

    public TEntity ToMockEntity()
    {
        TEntity mockEntity = (TEntity)Activator.CreateInstance(typeof(TEntity), nonPublic: true)!;
        foreach (IEntityKeyPropertyBinding<TEntity, TSelf> property in PropertyBindings)
        {
            property.SetValue(mockEntity, (TSelf)this);
        }
        return mockEntity;
    }
}
