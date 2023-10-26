using System.Linq.Expressions;
using System.Reflection;

namespace AndrejKrizan.DotNet.PropertyNavigations;

public class PropertyNavigation<T, TProperty> : IPropertyNavigation<T>
{
    // Properties
    public PropertyInfo Info { get; }
    public Expression<Func<T, TProperty>> Lambda { get; }

    // Computed properties
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
        Info = propertyInfosStack.Pop();

        propertyNavigationExpression = Expression.Property(parameter, Info);
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
    public static implicit operator PropertyNavigation<T, TProperty>(Expression<Func<T, TProperty>> lambda)
        => new(lambda);

    // Methods
    public PropertyNavigation<T, TProperty> ReplaceParameter(ParameterExpression parameter)
        => new(Lambda, parameter);

    public TProperty GetValue(T obj)
        => Func.Invoke(obj);


    public Expression CreateEqualsExpression(TProperty value)
        => Expression.Equal(Expression, Expression.Constant(value));

    public Expression<Func<T, bool>> CreateEqualsLambda(TProperty value)
        => Expression.Lambda<Func<T, bool>>(CreateEqualsExpression(value), Parameter);

    public void SetValue(T obj, TProperty value)
        => Info.SetValue(obj, value);

    public T CreateMockObject(TProperty value)
    {
        T mockObject = (T)Activator.CreateInstance(typeof(T), nonPublic: true)!;
        SetValue(mockObject, value);
        return mockObject;
    }

    // Interface implementations
    LambdaExpression IPropertyNavigation<T>.Lambda => Lambda;

    IPropertyNavigation<T> IPropertyNavigation<T>.ReplaceParameter(ParameterExpression parameter) => ReplaceParameter(parameter);
    object? IPropertyNavigation<T>.GetValue(T obj) => GetValue(obj);
    Expression IPropertyNavigation<T>.CreateEqualsExpression(object? value)
        => CreateEqualsExpression(
            value is TProperty propertyValue 
                ? propertyValue
                : throw new ArgumentException($"The value must be of {typeof(TProperty).Name} type.", nameof(value))
        );
}
