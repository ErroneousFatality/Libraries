using System.Linq.Expressions;
using System.Reflection;

namespace AndrejKrizan.DotNet.ValueObjects.PropertyNavigations;

public class PropertyNavigation<T, TProperty> : IPropertyNavigation<T>
{
    // Properties
    public Type Type => typeof(TProperty);
    public PropertyInfo PropertyInfo { get; }
    public Expression<Func<T, TProperty>> Lambda { get; }
    public Expression Expression => Lambda.Body;
    public ParameterExpression Parameter => Lambda.Parameters[0];

    public Func<T, TProperty> Func => _func ??= Lambda.Compile();
    private Func<T, TProperty>? _func;

    // Constructors

    /// <remarks>The only unary operator allowed inside the property navigation expression is <see cref="ExpressionType.Convert"/>.</remarks>
    public PropertyNavigation(Expression<Func<T, TProperty>> lambda)
        : this(
              lambda.Parameters.Count == 1
                ? lambda.Parameters[0]
                : throw new ArgumentException("The lambda must have exactly one parameter.", nameof(lambda)),
              lambda.Body
        )
    { }

    /// <remarks>The only unary operator allowed inside the property navigation expression is <see cref="ExpressionType.Convert"/>.</remarks>
    public PropertyNavigation(Expression<Func<T, TProperty>> lambda, ParameterExpression parameter)
        : this(parameter, lambda.Body) { }

    /// <remarks>The only unary operator allowed inside the property navigation expression is <see cref="ExpressionType.Convert"/>.</remarks>
    public PropertyNavigation(ParameterExpression parameter, Expression expression)
    {
        if (expression.Type != typeof(TProperty))
        {
            throw new ArgumentException($"The property navigation expression's target property type ({expression.Type}) does not match the property type ({typeof(TProperty)}).", nameof(expression));
        }
        if (parameter.Type != typeof(T))
        {
            throw new ArgumentException($"The parameter expression's type ({parameter.Type}) does not match the object type ({typeof(T)}).", nameof(parameter));
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
        Lambda = Expression.Lambda<Func<T, TProperty>>(propertyNavigationExpression, parameter);
    }

    // Conversions
    public static implicit operator PropertyNavigation<T, TProperty>(Expression<Func<T, TProperty>> propertySelector)
        => new(propertySelector);

    // Methods
    public PropertyNavigation<T, TProperty> ReplaceParameter(ParameterExpression parameter)
        => new(Lambda, parameter);

    public TProperty GetValue(T obj)
        => Func.Invoke(obj);


    public Expression ToEqualsExpression(TProperty value)
        => Expression.Equal(Expression, Expression.Constant(value));

    public Expression<Func<T, bool>> ToEqualsLambda(TProperty value)
        => Expression.Lambda<Func<T, bool>>(ToEqualsExpression(value), Parameter);

    public void SetValue(T obj, TProperty value)
        => PropertyInfo.SetValue(obj, value);

    public T ToMockObject(TProperty value)
    {
        T mockObject = (T)Activator.CreateInstance(typeof(T), nonPublic: true)!;
        SetValue(mockObject, value);
        return mockObject;
    }

    // Interface implementations
    LambdaExpression IPropertyNavigation<T>.Lambda => Lambda;

    IPropertyNavigation<T> IPropertyNavigation<T>.ReplaceParameter(ParameterExpression parameter) => ReplaceParameter(parameter);
    object? IPropertyNavigation<T>.GetValue(T obj) => GetValue(obj);
    Expression IPropertyNavigation<T>.ToEqualsExpression(object? value)
        => ToEqualsExpression(value is TProperty propertyValue ? propertyValue
            : throw new ArgumentException($"The value must be of {typeof(TProperty).Name} type.", nameof(value))
        );
}

public class PropertyNavigation<T> : IPropertyNavigation<T>
{
    // Properties
    public Type Type => Expression.Type;
    public PropertyInfo PropertyInfo { get; }
    public LambdaExpression Lambda { get; }
    public Expression Expression => Lambda.Body;
    public ParameterExpression Parameter => Lambda.Parameters[0];

    public Delegate Delegate => _delegate ??= Lambda.Compile();
    private Delegate? _delegate;

    // Constructors

    /// <remarks>The only unary operator allowed inside the property navigation expression is <see cref="ExpressionType.Convert"/>.</remarks>
    public PropertyNavigation(LambdaExpression lambda)
        : this(
              lambda.Body,
              lambda.Parameters.Count == 1
                ? lambda.Parameters[0]
                : throw new ArgumentException("The lambda must have exactly one parameter.", nameof(lambda))
        )
    { }

    /// <remarks>The only unary operator allowed inside the property navigation expression is <see cref="ExpressionType.Convert"/>.</remarks>
    public PropertyNavigation(LambdaExpression lambda, ParameterExpression parameter)
        : this(lambda.Body, parameter) { }

    /// <remarks>The only unary operator allowed inside the property navigation expression is <see cref="ExpressionType.Convert"/>.</remarks>
    public PropertyNavigation(Expression expression, ParameterExpression parameter)
    {
        if (parameter.Type != typeof(T))
        {
            throw new ArgumentException($"The parameter expression's type ({parameter.Type}) does not match the object type ({typeof(T)}).", nameof(parameter));
        }

        Stack<Type> conversionTypes = new();
        while (expression is UnaryExpression unaryExpression)
        {
            if (unaryExpression.NodeType != ExpressionType.Convert)
            {
                throw new ArgumentException($"Only conversion unary expressions are allowed inside a property nagivation expression. ({unaryExpression})", nameof(expression));
            }
            conversionTypes.Push(unaryExpression.Type);
            expression = unaryExpression.Operand;
        }

        Stack<PropertyInfo> propertyInfosStack = new();
        while (expression is not ParameterExpression)
        {
            if (expression is not MemberExpression memberExpression ||
                memberExpression.Member is not PropertyInfo propertyInfo ||
                memberExpression.Expression == null
            )
            {
                throw new ArgumentException($"The expression ({expression}) is not a property navigation expression.", nameof(expression));
            }
            propertyInfosStack.Push(propertyInfo);
            expression = memberExpression.Expression;
        }

        if (propertyInfosStack.Count < 1)
        {
            throw new ArgumentException($"The expression ({expression}) is not a property navigation expression.", nameof(expression));
        }
        PropertyInfo = propertyInfosStack.Pop();

        expression = Expression.Property(parameter, PropertyInfo);
        while (propertyInfosStack.Count > 0)
        {
            expression = Expression.Property(expression, propertyInfosStack.Pop());
        }
        while (conversionTypes.Count > 0)
        {
            expression = Expression.Convert(expression, conversionTypes.Pop());
        }
        Lambda = Expression.Lambda(expression, parameter);
    }

    // Conversions
    public static implicit operator PropertyNavigation<T>(LambdaExpression lambda)
        => new(lambda);

    // Methods
    public PropertyNavigation<T> ReplaceParameter(ParameterExpression parameter)
        => new(Lambda, parameter);

    public object? GetValue(T obj)
        => Delegate.DynamicInvoke(obj);

    public Expression ToEqualsExpression(object? value)
        => Expression.Equal(Expression, Expression.Constant(value));

    public Expression<Func<T, bool>> ToEqualsLambda(object? value)
        => Expression.Lambda<Func<T, bool>>(ToEqualsExpression(value), Parameter);

    public void SetValue(T obj, object? value)
        => PropertyInfo.SetValue(obj, value);

    // Interface implementations
    IPropertyNavigation<T> IPropertyNavigation<T>.ReplaceParameter(ParameterExpression parameter) => ReplaceParameter(parameter);
}
