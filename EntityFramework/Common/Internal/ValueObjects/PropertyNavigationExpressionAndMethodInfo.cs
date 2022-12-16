using System.Linq.Expressions;
using System.Reflection;

namespace AndrejKrizan.EntityFramework.Common.Internal.ValueObjects
{
    internal class PropertyNavigationExpressionAndMethodInfo<TEntity, TProperty>
        where TEntity : class
    {
        // Properties
        public PropertyNavigationExpression<TEntity, TProperty?> PropertyNavigationExpression { get; }
        public MethodInfo MethodInfo { get; }

        // Constructors

        public PropertyNavigationExpressionAndMethodInfo(
            PropertyNavigationExpression<TEntity, TProperty?> propertyNavigationExpression,
            MethodInfo methodInfo
        )
        {
            PropertyNavigationExpression = propertyNavigationExpression;
            MethodInfo = methodInfo;
        }

        /// <summary>
        ///     Example of a property predicate navigation expression: (TEntity entity) => entity.Property1...PropertyN.BoolMethod
        /// </summary>
        /// <exception cref="ArgumentException">
        ///     Expression must point to a method belonging to a property.
        ///     The property navigation expression's target property type does not match the property type.
        ///     The parameter expression's type does not match the entity type.
        ///     Only conversion unary expressions are allowed inside a property nagivation expression.
        ///     The expression is not a property navigation expression.
        /// </exception>
        public PropertyNavigationExpressionAndMethodInfo(
            Expression<Func<TEntity, Func<TProperty, bool>>> propertyPredicateMethodNavigationLambda,
            ParameterExpression parameterExpression
        )
            : this(propertyPredicateMethodNavigationLambda.Body, parameterExpression) { }

        /// <summary>
        ///     Example of a property predicate navigation expression: (TEntity entity) => entity.Property1...PropertyN.BoolMethod
        /// </summary>
        /// <exception cref="ArgumentException">
        ///     Expression must point to a method belonging to a property.
        ///     The property navigation expression's target property type does not match the property type.
        ///     The parameter expression's type does not match the entity type.
        ///     Only conversion unary expressions are allowed inside a property nagivation expression.
        ///     The expression is not a property navigation expression.
        /// </exception>
        public PropertyNavigationExpressionAndMethodInfo(
            Expression propertyPredicateMethodNavigationExpression,
            ParameterExpression parameterExpression
        )
        {
            Expression expression = propertyPredicateMethodNavigationExpression;
            if (expression is UnaryExpression unaryExpression && unaryExpression.NodeType == ExpressionType.Convert)
            {
                expression = unaryExpression.Operand;
            }
            if (expression is MethodCallExpression methodCallExpression &&
                methodCallExpression.Object is ConstantExpression constantExpression &&
                constantExpression.Value is MethodInfo methodInfo)
            {
                Expression? propertyNavigationExpression = methodCallExpression.Arguments
                    .Where(expression => expression.NodeType == ExpressionType.Convert || expression.NodeType == ExpressionType.MemberAccess)
                    .FirstOrDefault();
                if (propertyNavigationExpression != null)
                {
                    PropertyNavigationExpression = new(propertyNavigationExpression, parameterExpression);
                    MethodInfo = methodInfo;
                    return;
                }
            }
            throw new ArgumentException($"Expression {propertyPredicateMethodNavigationExpression} must point to a method belonging to a property.", nameof(propertyPredicateMethodNavigationExpression));
        }
    }
}
