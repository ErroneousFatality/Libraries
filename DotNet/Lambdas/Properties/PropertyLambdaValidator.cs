using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace AndrejKrizan.DotNet.Lambdas.Properties;

internal sealed class PropertyLambdaValidator : ExpressionVisitor
{
    // Fields
    private ParameterExpression? Parameter;
    private PropertyInfo? PropertyInfo;

    // Methods

    /// <remarks>Can be called multiple times with different inputs.</remarks>
    /// <exception cref="ArgumentException"></exception>
    public PropertyInfo ValidateAndGetPropertyInfo(LambdaExpression lambda)
    {
        if (lambda.Parameters.Count != 1)
            throw new ArgumentException("The property lambda must have exactly one parameter.", nameof(lambda));

        Visit(lambda.Body);

        if (Parameter == null)
            throw new ArgumentException("The property lambda's expression must use its parameter.", nameof(lambda));
        if (Parameter != lambda.Parameters[0])
            throw new ArgumentException("The property lambda's expression can not use a parameter which is not its own.", nameof(lambda));
        Parameter = null;

        if (PropertyInfo == null)
            throw new ArgumentException("The property lambda's expression must use a property accessor.", nameof(lambda));
        if (!PropertyInfo.PropertyType.IsAssignableTo(lambda.ReturnType))
            throw new ArgumentException($"The property lambda's expression targets a property whose type ({PropertyInfo.PropertyType}) is not assignable to the lambda's return type ({lambda.ReturnType}).", nameof(lambda));
        PropertyInfo _propertyInfo = PropertyInfo;
        PropertyInfo = null;

        return _propertyInfo;
    }

    // Method overrides
    /// <remarks>Do not call this directly, use <see cref="ValidateAndGetPropertyInfo(LambdaExpression)"/>.</remarks>
    /// <exception cref="ArgumentException"></exception>
    [return: NotNullIfNotNull(nameof(expression))]
    public override Expression? Visit(Expression? expression)
    {
        if (expression == null)
            goto Base;

        if (PropertyInfo == null)
        {
            if (expression.NodeType is not ExpressionType.MemberAccess)
                throw new ArgumentException("The property lambda's expression must end with a property accessor.", nameof(expression));
        }
        else if (Parameter == null)
        {
            if (expression.NodeType is not ExpressionType.Parameter)
                throw new ArgumentException("The property lambda's expression must start with the parameter.", nameof(expression));
        }
        else
        {
            throw new ArgumentException("The property lambda's expression must contain only the parameter and property accessor.", nameof(expression));
        }

    Base:
        Expression? _expression = base.Visit(expression);
        return _expression;
    }

    /// <exception cref="ArgumentException"></exception>
    protected override Expression VisitMember(MemberExpression member)
    {
        if (member.Member is not PropertyInfo propertyInfo)
            throw new ArgumentException("The proprety lambda's expression can not use member expressions other than property accessors.", nameof(member));
        PropertyInfo = propertyInfo;
        return base.VisitMember(member);
    }

    /// <exception cref="ArgumentException"></exception>
    protected override Expression VisitParameter(ParameterExpression parameter)
    {
        Parameter = parameter;
        return base.VisitParameter(parameter);
    }
}