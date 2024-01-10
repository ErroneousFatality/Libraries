using System.Linq.Expressions;
using System.Reflection;

namespace AndrejKrizan.DotNet.Lambdas.Properties;
public class PropertyLambda<T, TProperty> : PropertyLambda
{
    // Properties
    public override Expression<Func<T, TProperty>> Lambda
        => (Expression<Func<T, TProperty>>)base.Lambda;

    // Constructors
    public PropertyLambda(Expression<Func<T, TProperty>> lambda)
        : base(lambda) { }

    private PropertyLambda(Expression<Func<T, TProperty>> lambda, PropertyInfo propertyInfo)
        : base(lambda, propertyInfo) { }

    // Methods
    public override PropertyLambda<T, TProperty> ReplaceParameter(ParameterExpression parameter)
    {
        Expression<Func<T, TProperty>> lambda = Lambda.ReplaceParameter(parameter);
        PropertyLambda<T, TProperty> property = new(lambda, PropertyInfo);
        return property;
    }

    // Conversions
    public static implicit operator PropertyLambda<T, TProperty>(Expression<Func<T, TProperty>> lambda) => new(lambda);
    public static implicit operator Expression<Func<T, TProperty>>(PropertyLambda<T, TProperty> propertyLambda) => propertyLambda.Lambda;
}
