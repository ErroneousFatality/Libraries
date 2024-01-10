using System.Linq.Expressions;

namespace AndrejKrizan.DotNet.Lambdas.Navigations;
public class NavigationLambda<T, TProperty> : NavigationLambda
{
    // Properties
    public override Expression<Func<T, TProperty>> Lambda
        => (Expression<Func<T, TProperty>>)base.Lambda;

    public override Func<T, TProperty> Delegate
        => (Func<T, TProperty>)base.Delegate;

    // Constructors
    public NavigationLambda(Expression<Func<T, TProperty>> lambda)
        : base(lambda) { }

    private NavigationLambda(Expression<Func<T, TProperty>> lambda, bool validate)
        : base(lambda, validate) { }

    // Conversions
    public static implicit operator NavigationLambda<T, TProperty>(Expression<Func<T, TProperty>> lambda) => new(lambda);
    public static implicit operator Expression<Func<T, TProperty>>(NavigationLambda<T, TProperty> propertyLambda) => propertyLambda.Lambda;

    // Methods
    public TProperty Navigate(T source)
        => Delegate.Invoke(source);

    public override NavigationLambda<T, TProperty> ReplaceParameter(ParameterExpression parameter)
    {
        Expression<Func<T, TProperty>> lambda = Lambda.ReplaceParameter(parameter);
        NavigationLambda<T, TProperty> property = new(lambda, validate: false);
        return property;
    }

    public NavigationLambda<T, TResult> Extend<TResult>(NavigationLambda<TProperty, TResult> extension)
    {
        Expression<Func<T, TResult>> lambda = Lambda.Extend(extension.Lambda);
        NavigationLambda<T, TResult> navigation = new(lambda, validate: false);
        return navigation;
    }
}
