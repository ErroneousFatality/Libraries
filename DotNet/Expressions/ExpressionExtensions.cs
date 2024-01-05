using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace AndrejKrizan.DotNet.Expressions;

public static class ExpressionExtensions
{
    /// <summary>
    /// Creates a <see cref="BinaryExpression"/> that represents a conditional AND operation that evaluates the second operand only if it has to.
    /// </summary>
    /// <param name="left">An <see cref="Expression"/> to set the <see cref="BinaryExpression.Left"/> property equal to.</param>
    /// <param name="right">An <see cref="Expression"/> to set the <see cref="BinaryExpression.Right"/> property equal to.</param>
    /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="Expression.NodeType"/> property equal to <see cref="ExpressionType.AndAlso"/>
    /// and the <see cref="BinaryExpression.Left"/> and <see cref="BinaryExpression.Right"/> properties set to the specified values.</returns>
    public static BinaryExpression AndAlso(this Expression left, Expression right)
        => Expression.AndAlso(left, right);

    /// <summary>
    /// Creates a <see cref="BinaryExpression"/> that represents a conditional OR operation that evaluates the second operand only if it has to.
    /// </summary>
    /// <param name="left">An <see cref="Expression"/> to set the <see cref="BinaryExpression.Left"/> property equal to.</param>
    /// <param name="right">An <see cref="Expression"/> to set the <see cref="BinaryExpression.Right"/> property equal to.</param>
    /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="Expression.NodeType"/> property equal to <see cref="ExpressionType.OrElse"/>
    /// and the <see cref="BinaryExpression.Left"/> and <see cref="BinaryExpression.Right"/> properties set to the specified values.</returns>
    public static BinaryExpression OrElse(this Expression left, Expression right)
        => Expression.OrElse(left, right);

    #region ReplaceParameters
    public static TLambda ReplaceParameter<TLambda>(this TLambda source, Expression parameter)
        where TLambda : LambdaExpression
    {
        ReadOnlyCollection<ParameterExpression> sourceParameters = source.Parameters;
        if (sourceParameters.Count != 1)
        {
            throw new ArgumentException("The source lambda must have exactly one parameter.", nameof(source));
        }
        ParameterExpression oldParameter = sourceParameters[0];
        TLambda lambda = (TLambda)new ParameterReplacer(oldParameter, parameter).Visit(source);
        return lambda;
    }

    public static TLambda ReplaceParameters<TLambda>(this TLambda source, params Expression[] parameters)
        where TLambda : LambdaExpression
    {
        ReadOnlyCollection<ParameterExpression> sourceParameters = source.Parameters;
        if (parameters.Length > sourceParameters.Count)
        {
            throw new ArgumentException("There are more provided parameters than the source lambda parameters.", nameof(parameters));
        }
        Dictionary<ParameterExpression, Expression> parameterMappings = sourceParameters
            .Zip(parameters, (_old, _new) => new KeyValuePair<ParameterExpression, Expression>(_old, _new))
            .ToDictionary();
        TLambda lambda = (TLambda)new ParameterReplacer(parameterMappings).Visit(source);
        return lambda;
    }

    public static Expression ReplaceParameter(this Expression source, ParameterExpression parameter, Expression expression)
        => new ParameterReplacer(parameter, expression).Visit(source);

    public static Expression ReplaceParameters(this Expression source, IDictionary<ParameterExpression, Expression> mappings)
        => new ParameterReplacer(mappings).Visit(source);

    private class ParameterReplacer : ExpressionVisitor
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

        // Methods
        protected override Expression VisitParameter(ParameterExpression parameter)
            => Dictionary.TryGetValue(parameter, out Expression? expression)
                ? expression
                : parameter;
    }
    #endregion

    public static Expression<Func<TSource, TResult>> Apply<TSource, TIntermediate, TResult>(
        this Expression<Func<TSource, TIntermediate>> source,
        Expression<Func<TIntermediate, TResult>> lambda
    )
    {
        ParameterExpression parameter = source.Parameters[0];
        Expression combinedBody = lambda.Body.ReplaceParameter(lambda.Parameters[0], source.Body);
        Expression<Func<TSource, TResult>> combinedLambda = Expression.Lambda<Func<TSource, TResult>>(combinedBody, parameter);
        return combinedLambda;
    }
}
