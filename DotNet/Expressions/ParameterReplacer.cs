using System.Linq.Expressions;

namespace AndrejKrizan.DotNet.Expressions;
public sealed class ParameterReplacer : ExpressionVisitor
{
    // Properties
    private IDictionary<ParameterExpression, Expression> Dictionary { get; }

    // Constructors
    public ParameterReplacer(ParameterExpression parameter, Expression expression)
    {
        Dictionary = new Dictionary<ParameterExpression, Expression>
        {
            [parameter] = expression
        };
    }

    public ParameterReplacer(IDictionary<ParameterExpression, Expression> dictionary)
    {
        Dictionary = dictionary;
    }

    // Method overrides
    protected override Expression VisitParameter(ParameterExpression parameter)
        => Dictionary.TryGetValue(parameter, out Expression? expression)
            ? expression
            : parameter;
}
