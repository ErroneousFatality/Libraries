using System.Linq.Expressions;

namespace AndrejKrizan.DotNet.Extensions
{
    public static class ExpressionExtensions
    {
        /// <summary>
        ///     Creates a <see cref="BinaryExpression"/> that represents a conditional AND operation that evaluates the second operand only if the first operand evaluates to true.
        /// </summary>
        public static BinaryExpression AndAlso(this Expression left, Expression right)
            => Expression.AndAlso(left, right);
        public static BinaryExpression OrElse(this Expression left, Expression right)
            => Expression.OrElse(left, right);

        public static Expression<Func<X, Y>> ReplaceParameters<X, Y>(this Expression<Func<X, Y>> lambda, params ParameterExpression[] parameterExpressions)
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
            lambda = (Expression<Func<X, Y>>)new ParameterReplacer(dictionary).Visit(lambda);
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
    }
}
