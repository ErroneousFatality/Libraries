using System.Linq.Expressions;
using System.Reflection;

namespace AndrejKrizan.EntityFramework.Common.Extensions.Lambda;

internal static class ExpressionExtensions
{
    public static PropertyInfo GetPropertyInfo(this Expression expression)
    {
        if (expression is MemberExpression memberExpression &&
            memberExpression.Expression is ParameterExpression &&
            memberExpression.Member is PropertyInfo propertyInfo)
        {
            return propertyInfo;
        }
        throw new ArgumentException($"Expression \"{expression}\" does not represent a property.", nameof(expression));
    }
}
