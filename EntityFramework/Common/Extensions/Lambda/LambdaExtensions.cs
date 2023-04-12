using System.Linq.Expressions;
using System.Reflection;

namespace AndrejKrizan.EntityFramework.Common.Extensions.Lambda;

internal static class LambdaExtensions
{
    /// <exception cref="InvalidOperationException">
    ///     Lambda expression does not directly express the entity's key property.
    /// </exception>
    public static PropertyInfo GetEntityKeyPropertyInfo<TEntity, TKey>(this Expression<Func<TEntity, TKey>> entityKeyLambda)
        where TEntity : class
    {
        if (entityKeyLambda.Body is MemberExpression memberExpression &&
            memberExpression.Expression is ParameterExpression &&
            memberExpression.Member is PropertyInfo propertyInfo)
        {
            return propertyInfo;
        }
        throw new InvalidOperationException($"Lambda expression \"{entityKeyLambda}\" does not directly represent the entity's key property.");
    }
}
