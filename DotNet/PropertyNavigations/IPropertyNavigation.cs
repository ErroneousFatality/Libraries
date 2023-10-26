using System.Linq.Expressions;
using System.Reflection;

namespace AndrejKrizan.DotNet.PropertyNavigations;

public interface IPropertyNavigation<T>
{
    // Properties
    PropertyInfo Info { get; }
    LambdaExpression Lambda { get; }
    Expression Expression { get; }
    ParameterExpression Parameter { get; }

    // Methods
    IPropertyNavigation<T> ReplaceParameter(ParameterExpression parameter);
    object? GetValue(T obj);
    Expression CreateEqualsExpression(object? value);
}
