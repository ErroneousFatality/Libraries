using System.Linq.Expressions;

using AndrejKrizan.DotNet.Expressions;

namespace AndrejKrizan.DotNet.NavigationLambdas;
public sealed class NavigationLambda
{
    // Properties
    public LambdaExpression Lambda { get; private init; }

    // Computed properties
    public ParameterExpression Parameter => Lambda.Parameters[0];
    public Expression Expression => Lambda.Body;
    public Type ReturnType => Lambda.ReturnType;

    public Delegate Delegate => _delegate ??= Lambda.Compile();
    private Delegate? _delegate;

    // Constructors
    public NavigationLambda(LambdaExpression lambda)
        : this(lambda, validate: true) { }

    private NavigationLambda(LambdaExpression lambda, bool validate)
    {
        if (validate)
        {
            NavigationLambdaValidator validator = new();
            validator.Validate(lambda);
        }
        Lambda = lambda;
    }

    // Conversions
    public static implicit operator NavigationLambda(LambdaExpression lambda) => new(lambda);
    public static implicit operator LambdaExpression(NavigationLambda navigationLambda) => navigationLambda.Lambda;

    // Methods
    public NavigationLambda Combine(NavigationLambda other)
    {
        if (Lambda.ReturnType != other.Parameter.Type)
            throw new ArgumentException("The navigation lambdas can only be combined if the firt's return type is equal to the second's parameter type.", nameof(other));
        Expression expression = other.Expression.ReplaceParameter(other.Parameter, Expression);
        LambdaExpression lambda = Expression.Lambda(expression, Parameter);
        NavigationLambda navigation = new(lambda, validate: false);
        return navigation;
    }

    public override string ToString()
        => Lambda.ToString();
}
