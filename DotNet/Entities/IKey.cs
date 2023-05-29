using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

using AndrejKrizan.DotNet.Extensions;
using AndrejKrizan.DotNet.ValueObjects.PropertyBindings;

[assembly: InternalsVisibleTo("AndrejKrizan.EntityFramework.Common")]
namespace AndrejKrizan.DotNet.Entities;
public interface IKey<TEntity, TSelf>
    where TEntity : class
    where TSelf: struct, IKey<TEntity, TSelf>
{
    // Static properties
    static abstract Expression<Func<TEntity, TSelf>> Lambda { get; }
    
    internal static readonly ParameterExpression EntityParameter;
    internal static readonly ImmutableArray<IPropertyBinding<TEntity, TSelf>> PropertyBindings;
    static IKey()
    {
        Expression<Func<TEntity, TSelf>> lambda = TSelf.Lambda;
        EntityParameter = lambda.Parameters.Single();

        const string errorMessage = $"The {nameof(Lambda)} expression must use an object initializer. For example: entity => new EntityKey {{ A = entity.A, B = entity.B}}";
        if (lambda.Body is not MemberInitExpression initialization)
        {
            throw new Exception(errorMessage);
        }
        ParameterExpression keyParameter = Expression.Parameter(typeof(TSelf), "key");
        PropertyBindings = initialization.Bindings.Convert((MemberBinding memberBinding) =>
        {
            if (memberBinding.Member is not PropertyInfo keyProperty || memberBinding is not MemberAssignment keyAssignment)
            {
                throw new Exception(errorMessage);
            }
            Expression entityProperty = keyAssignment.Expression;
            Type bindingType = typeof(PropertyBinding<,,>).MakeGenericType(typeof(TEntity), typeof(TSelf), keyProperty.PropertyType);
            IPropertyBinding<TEntity, TSelf> propertyBinding = (IPropertyBinding<TEntity, TSelf>)Activator.CreateInstance(bindingType, EntityParameter, entityProperty, keyParameter, keyProperty)!;
            return propertyBinding;
        });
    }
}
