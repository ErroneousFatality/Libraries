using System.Linq.Expressions;

namespace AndrejKrizan.DotNet.Lambdas.Navigations;
public class NavigationLambda
{
    // Properties
    public virtual LambdaExpression Lambda { get; }

    // Computed properties
    public ParameterExpression Parameter => Lambda.Parameters[0];
    public Expression Expression => Lambda.Body;
    public Type ReturnType => Lambda.ReturnType;

    public virtual Delegate Delegate => _delegate ??= Lambda.Compile();
    private Delegate? _delegate;

    // Constructors
    public NavigationLambda(LambdaExpression lambda)
        : this(lambda, validate: true) { }

    protected NavigationLambda(LambdaExpression lambda, bool validate)
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

    public override string ToString()
        => Lambda.ToString();

    public virtual object? Navigate(object source)
        => Delegate.DynamicInvoke(source);

    public virtual NavigationLambda ReplaceParameter(ParameterExpression parameter)
    {
        LambdaExpression lambda = Lambda.ReplaceParameter(parameter);
        NavigationLambda navigation = new(lambda, validate: false);
        return navigation;
    }

    public virtual NavigationLambda Extend(NavigationLambda extension)
    {
        LambdaExpression lambda = Lambda.Extend(extension.Lambda);
        NavigationLambda navigation = new(lambda, validate: false);
        return navigation;
    }
}