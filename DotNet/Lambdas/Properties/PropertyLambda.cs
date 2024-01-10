using System.Linq.Expressions;
using System.Reflection;

namespace AndrejKrizan.DotNet.Lambdas.Properties;
public class PropertyLambda
{
    // Properties
    public virtual LambdaExpression Lambda { get; }
    public PropertyInfo PropertyInfo { get; }

    // Computed properties
    public ParameterExpression Parameter => Lambda.Parameters[0];
    public Expression Expression => Lambda.Body;

    // Constructors
    public PropertyLambda(LambdaExpression lambda)
    {
        PropertyLambdaVisitor validator = new();
        PropertyInfo propertyInfo = validator.ValidateAndGetPropertyInfo(lambda);

        Lambda = lambda;
        PropertyInfo = propertyInfo;
    }

    protected PropertyLambda(LambdaExpression lambda, PropertyInfo propertyInfo)
    {
        Lambda = lambda;
        PropertyInfo = propertyInfo;
    }

    // Conversions
    public static implicit operator PropertyLambda(LambdaExpression lambda) => new(lambda);
    public static implicit operator LambdaExpression(PropertyLambda propertyLambda) => propertyLambda.Lambda;

    // Methods
    public object? GetValue(object source)
    {
        ValidateSource(source);
        object? value = PropertyInfo.GetValue(source);
        return value;
    }

    public void SetValue(object source, object? value)
    {
        ValidateSource(source);
        ValidateValue(value);
        PropertyInfo.SetValue(source, value);
    }

    public override string ToString()
        => Lambda.ToString();

    public virtual PropertyLambda ReplaceParameter(ParameterExpression parameter)
    {
        LambdaExpression lambda = Lambda.ReplaceParameter(parameter);
        PropertyLambda property = new(lambda, PropertyInfo);
        return property;
    }

    public BinaryExpression ToEqualsExpression(object? value)
    {
        ValidateValue(value);
        ConstantExpression _value = Expression.Constant(value);
        BinaryExpression expression = Expression.Equal(Expression, _value);
        return expression;
    }

    public LambdaExpression ToEqualsLambda(object? value)
    {
        BinaryExpression expression = ToEqualsExpression(value);
        LambdaExpression lambda = Expression.Lambda(expression, Parameter);
        return lambda;
    }

    public Expression<Func<T, bool>> ToEqualsLambda<T>(object? value)
    {
        if (!typeof(T).IsAssignableTo(Parameter.Type))
        {
            throw new InvalidCastException($"The given type ({nameof(T)} = {typeof(T)}) is not assignable to lambda's paramter type ({Parameter.Type}).");
        }
        BinaryExpression expression = ToEqualsExpression(value);
        Expression<Func<T, bool>> lambda = Expression.Lambda<Func<T, bool>>(expression, Parameter);
        return lambda;
    }

    public void ValidateSource(object source)
    {
        if (!Parameter.Type.IsInstanceOfType(source))
        {
            throw new ArgumentException($"The source object ({source}) must be an instance of the property lambda's parameter type ({Parameter.Type}).", nameof(source));
        }
    }

    public void ValidateValue(object? value)
    {
        if (value != null && !PropertyInfo.PropertyType.IsInstanceOfType(value))
        {
            throw new ArgumentException($"The value ({value}) must be an instance of the property type ({PropertyInfo.PropertyType}).", nameof(value));
        }
    }
}
