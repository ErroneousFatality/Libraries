using System.Linq.Expressions;
using System.Reflection;

namespace AndrejKrizan.DotNet.Utilities.Func;
public class MethodInfoFinder : ExpressionVisitor
{
    // Methods
    public MethodInfo FindIn(Expression expression)
    {
        Visit(expression);
        return MethodInfo ?? throw new ArgumentException($"The expression {expression} contains no method information.");
    }

    // Protected methods
    protected override Expression VisitConstant(ConstantExpression node)
    {
        if (node.Value is MethodInfo methodInfo)
        {
            if (MethodInfo != null)
            {
                throw new ArgumentException("The expression contains multiple method informations.");
            }
            MethodInfo = methodInfo;
        }
        return base.VisitConstant(node);
    }

    // Private fields
    private MethodInfo? MethodInfo;
}
