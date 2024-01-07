using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace AndrejKrizan.DotNet.NavigationLambdas;

internal class NavigationLambdaValidator : ExpressionVisitor
{
    // Fields
    private ParameterExpression? Parameter;
    private bool WasParameterFound;

    // Constructors
    public NavigationLambdaValidator() { }

    // Methods

    /// <remarks>Can be called multiple times with different inputs.</remarks>
    /// <exception cref="ArgumentException"></exception>
    public void Validate(LambdaExpression lambda)
    {
        if (lambda.Parameters.Count != 1)
            throw new ArgumentException("The navigation lambda must have exactly one parameter.", nameof(lambda));
        Parameter = lambda.Parameters[0];

        Visit(lambda.Body);

        if (!WasParameterFound)
            throw new ArgumentException("The navigation lambda's expression must use the parameter.", nameof(lambda));

        Parameter = null;
        WasParameterFound = false;
    }

    // Method overrides
    /// <remarks>Do not call this directly, use <see cref="Validate(LambdaExpression)"/>.</remarks>
    /// <exception cref="ArgumentException"></exception>
    [return: NotNullIfNotNull(nameof(expression))]
    public override Expression? Visit(Expression? expression)
    {
        if (expression == null)
            goto Base;

        if (WasParameterFound)
            throw new ArgumentException("The navigation lambda's expression must start with the parameter.", nameof(expression));

        if (expression.NodeType is not (
            ExpressionType.Convert or
            ExpressionType.ConvertChecked or
            ExpressionType.MemberAccess or
            ExpressionType.Parameter or
            ExpressionType.TypeAs
        ))
            throw new ArgumentException("The navigation lambda's expression can only use the parameter, property accessors and casts.");

        Base:
        Expression? _expression = base.Visit(expression);
        return _expression;
    }

    /// <exception cref="ArgumentException"></exception>
    protected override Expression VisitMember(MemberExpression member)
    {
        if (member.Member.MemberType != MemberTypes.Property)
            throw new ArgumentException("The navigation lambda's expression can not use member expressions other than property accessors..", nameof(member));
        return base.VisitMember(member);
    }

    /// <exception cref="ArgumentException"></exception>
    protected override Expression VisitParameter(ParameterExpression parameter)
    {
        if (Parameter != parameter)
            throw new ArgumentException("The navigation lambda's expression can not use a parameter which is not its own.", nameof(parameter));
        if (WasParameterFound)
            throw new ArgumentException("The navigation lambda's expression can not use the parameter more tha once.", nameof(parameter));
        WasParameterFound = true;
        return base.VisitParameter(parameter);
    }
}