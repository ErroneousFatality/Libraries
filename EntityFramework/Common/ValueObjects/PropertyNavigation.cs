using System.Linq.Expressions;
using System.Reflection;

using AndrejKrizan.DotNet;

namespace AndrejKrizan.EntityFramework.Common.ValueObjects;

public interface IPropertyNavigation<TObject>
{
    // Properties
    Type Type { get; }
    PropertyInfo PropertyInfo { get; }
    LambdaExpression Lambda { get; }
    Expression Expression { get; }
    ParameterExpression Parameter { get; }

    // Methods
    IPropertyNavigation<TObject> ReplaceParameter(ParameterExpression parameter);
    object? GetValue(TObject obj);
    Expression ToEqualsExpression(object? value);
}

public class PropertyNavigation<TObject, TProperty> : IPropertyNavigation<TObject>
{
    // Properties
    public Type Type => typeof(TProperty);
    public PropertyInfo PropertyInfo { get; }
    public Expression<Func<TObject, TProperty>> Lambda { get; }
    public Expression Expression => Lambda.Body;
    public ParameterExpression Parameter => Lambda.Parameters[0];

    public Func<TObject, TProperty> Func => _func ??= Lambda.Compile();
    private Func<TObject, TProperty>? _func;

    // Constructors

    /// <remarks>The only unary operator allowed inside the property navigation expression is <see cref="ExpressionType.Convert"/>.</remarks>
    public PropertyNavigation(Expression<Func<TObject, TProperty>> lambda)
        : this(
              lambda.Body,
              lambda.Parameters.Count == 1
                ? lambda.Parameters[0]
                : throw new ArgumentException("The lambda must have exactly one parameter.", nameof(lambda))
        )
    { }

    /// <remarks>The only unary operator allowed inside the property navigation expression is <see cref="ExpressionType.Convert"/>.</remarks>
    public PropertyNavigation(Expression<Func<TObject, TProperty>> lambda, ParameterExpression parameter)
        : this(lambda.Body, parameter) { }

    /// <remarks>The only unary operator allowed inside the property navigation expression is <see cref="ExpressionType.Convert"/>.</remarks>
    public PropertyNavigation(Expression expression, ParameterExpression parameter)
    {
        if (expression.Type != typeof(TProperty))
        {
            throw new ArgumentException($"The property navigation expression's target property type ({expression.Type}) does not match the property type ({typeof(TProperty)}).", nameof(expression));
        }
        if (parameter.Type != typeof(TObject))
        {
            throw new ArgumentException($"The parameter expression's type ({parameter.Type}) does not match the object type ({typeof(TObject)}).", nameof(parameter));
        }

        Expression propertyNavigationExpression = expression;

        Stack<Type> conversionTypes = new();
        while (propertyNavigationExpression is UnaryExpression unaryExpression)
        {
            if (unaryExpression.NodeType != ExpressionType.Convert)
            {
                throw new ArgumentException($"Only conversion unary expressions are allowed inside a property nagivation expression. ({unaryExpression})", nameof(expression));
            }
            conversionTypes.Push(unaryExpression.Type);
            propertyNavigationExpression = unaryExpression.Operand;
        }

        Stack<PropertyInfo> propertyInfosStack = new();
        while (propertyNavigationExpression is not ParameterExpression)
        {
            if (propertyNavigationExpression is not MemberExpression memberExpression ||
                memberExpression.Member is not PropertyInfo propertyInfo ||
                memberExpression.Expression == null
            )
            {
                throw new ArgumentException($"The expression ({expression}) is not a property navigation expression.", nameof(expression));
            }
            propertyInfosStack.Push(propertyInfo);
            propertyNavigationExpression = memberExpression.Expression;
        }

        if (propertyInfosStack.Count < 1)
        {
            throw new ArgumentException($"The expression ({expression}) is not a property navigation expression.", nameof(expression));
        }
        PropertyInfo = propertyInfosStack.Pop();

        propertyNavigationExpression = Expression.Property(parameter, PropertyInfo);
        while (propertyInfosStack.Count > 0)
        {
            propertyNavigationExpression = Expression.Property(propertyNavigationExpression, propertyInfosStack.Pop());
        }
        while (conversionTypes.Count > 0)
        {
            propertyNavigationExpression = Expression.Convert(propertyNavigationExpression, conversionTypes.Pop());
        }
        Lambda = Expression.Lambda<Func<TObject, TProperty>>(propertyNavigationExpression, parameter);
    }

    // Methods
    public PropertyNavigation<TObject, TProperty> ReplaceParameter(ParameterExpression parameter)
        => new(Lambda, parameter);

    public TProperty GetValue(TObject obj)
        => Func.Invoke(obj);


    public Expression ToEqualsExpression(TProperty value)
        => Expression.Equal(Expression, Expression.Constant(value));

    public Expression<Func<TObject, bool>> ToEqualsLambda(TProperty value)
        => Expression.Lambda<Func<TObject, bool>>(ToEqualsExpression(value), Parameter);

    public void SetValue(TObject obj, TProperty value)
        => PropertyInfo.SetValue(obj, value);

    public TObject ToMockObject(TProperty value)
    {
        TObject mockObject = (TObject)Activator.CreateInstance(typeof(TObject), nonPublic: true)!;
        SetValue(mockObject, value);
        return mockObject;
    }

    // Interface implementations
    LambdaExpression IPropertyNavigation<TObject>.Lambda => Lambda;

    IPropertyNavigation<TObject> IPropertyNavigation<TObject>.ReplaceParameter(ParameterExpression parameter) => ReplaceParameter(parameter);
    object? IPropertyNavigation<TObject>.GetValue(TObject obj) => GetValue(obj);
    Expression IPropertyNavigation<TObject>.ToEqualsExpression(object? value)
        => ToEqualsExpression(value is TProperty propertyValue ? propertyValue 
            : throw new ArgumentException($"The value must be of {typeof(TProperty).Name} type.", nameof(value))
        );
}
