using System.Linq.Expressions;
using System.Reflection;

namespace AndrejKrizan.DotNet.ValueObjects.PropertyNavigations;

public interface IPropertyNavigation<T>
{
    // Properties
    Type Type { get; }
    PropertyInfo PropertyInfo { get; }
    LambdaExpression Lambda { get; }
    Expression Expression { get; }
    ParameterExpression Parameter { get; }

    // Methods
    IPropertyNavigation<T> ReplaceParameter(ParameterExpression parameter);
    object? GetValue(T obj);
    Expression ToEqualsExpression(object? value);
}
