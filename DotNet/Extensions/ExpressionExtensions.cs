using System.Linq.Expressions;

namespace AndrejKrizan.DotNet.Extensions;

public static class ExpressionExtensions
{
    /// <summary>
    /// Creates a <see cref="BinaryExpression"/> that represents a conditional AND operation that evaluates the second operand only if it has to.
    /// </summary>
    /// <param name="left">An <see cref="Expression"/> to set the <see cref="BinaryExpression.Left"/> property equal to.</param>
    /// <param name="right">An <see cref="Expression"/> to set the <see cref="BinaryExpression.Right"/> property equal to.</param>
    /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="NodeType"/> property equal to <see cref="ExpressionType.AndAlso"/>
    /// and the <see cref="BinaryExpression.Left"/> and <see cref="BinaryExpression.Right"/> properties set to the specified values.</returns>
    public static BinaryExpression AndAlso(this Expression left, Expression right)
        => Expression.AndAlso(left, right);

    /// <summary>
    /// Creates a <see cref="BinaryExpression"/> that represents a conditional OR operation that evaluates the second operand only if it has to.
    /// </summary>
    /// <param name="left">An <see cref="Expression"/> to set the <see cref="BinaryExpression.Left"/> property equal to.</param>
    /// <param name="right">An <see cref="Expression"/> to set the <see cref="BinaryExpression.Right"/> property equal to.</param>
    /// <returns>A <see cref="BinaryExpression"/> that has the <see cref="NodeType"/> property equal to <see cref="ExpressionType.OrElse"/>
    /// and the <see cref="BinaryExpression.Left"/> and <see cref="BinaryExpression.Right"/> properties set to the specified values.</returns>
    public static BinaryExpression OrElse(this Expression left, Expression right)
        => Expression.OrElse(left, right);

    #region UnwrapConversion
    public static Expression UnwrapConversion<TConvert, T>(this Expression expression)
    {
        if (expression is UnaryExpression convert && convert.NodeType == ExpressionType.Convert && convert.Type == typeof(TConvert))
        {
            Expression innerExpression = convert.Operand;
            if (innerExpression.Type == typeof(T))
            {
                return innerExpression;
            }
        }
        throw new ArgumentException($"Expression is not a conversion from {nameof(T)} to {nameof(TConvert)}.", nameof(expression));
    }

    public static Expression<Func<TParameter, T>> UnwrapConversion<TParameter, TConvert, T>(this Expression<Func<TParameter, TConvert>> lambda)
        => Expression.Lambda<Func<TParameter, T>>(lambda.Body.UnwrapConversion<TConvert, T>(), lambda.Parameters);


    public static Expression UnwrapConversionsUntilType(this Expression expression, Type type)
    {
        while (expression is UnaryExpression conversion && conversion.NodeType == ExpressionType.Convert)
        {
            Expression innerExpression = conversion.Operand;
            if (innerExpression.Type == type)
            {
                return innerExpression;
            }
        }
        throw new ArgumentException($"No conversion to {type.Name} found.", nameof(expression));
    }
    public static Expression UnwrapConversionsUntilType<T>(this Expression expression)
        => expression.UnwrapConversionsUntilType(typeof(T));

    public static Expression<Func<TParameter, T>> UnwrapConversionsUntilType<TParameter, TCurrent, T>(this Expression<Func<TParameter, TCurrent>> lambda)
        => Expression.Lambda<Func<TParameter, T>>(lambda.Body.UnwrapConversionsUntilType<T>(), lambda.Parameters);
    #endregion UnwrapConversion

    #region ReplaceParameters
    public static TLambda ReplaceParameters<TLambda>(this TLambda lambda, params ParameterExpression[] parameterExpressions)
        where TLambda: LambdaExpression
    {
        if (parameterExpressions.Length > lambda.Parameters.Count)
        {
            throw new ArgumentException("There are more parameter expressions than the lambda has parameters.", nameof(parameterExpressions));
        }
        Dictionary<ParameterExpression, ParameterExpression> dictionary = new(parameterExpressions.Length);
        for (int i = 0; i < parameterExpressions.Length; i++)
        {
            dictionary.Add(lambda.Parameters[i], parameterExpressions[i]);
        }
        lambda = (TLambda)new ParameterReplacer(dictionary).Visit(lambda);
        return lambda;
    }

    class ParameterReplacer : ExpressionVisitor
    {
        // Properties
        private IDictionary<ParameterExpression, ParameterExpression> Dictionary { get; }

        // Constructors
        public ParameterReplacer(ParameterExpression oldParameter, ParameterExpression newParameter)
        {
            Dictionary = new Dictionary<ParameterExpression, ParameterExpression>
            {
                [oldParameter] = newParameter
            };
        }

        public ParameterReplacer(IDictionary<ParameterExpression, ParameterExpression> dictionary)
        {
            Dictionary = dictionary;
        }

        // Methods
        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (Dictionary.TryGetValue(node, out ParameterExpression? parameter))
            {
                return parameter;
            }
            return node;
        }
    }
    #endregion
}
