using System.Linq.Expressions;
using System.Reflection;

namespace AndrejKrizan.EntityFramework.Common.Extensions.Lambda
{
    internal class PropertyNavigationExpression<TEntity, TProperty>
        where TEntity : class
    {
        // Properties
        public Expression Expression { get; }

        // Constructors

        /// <exception cref="ArgumentException">
        ///     The property navigation expression's target property type does not match the property type.
        ///     The parameter expression's type does not match the entity type.
        ///     Only conversion unary expressions are allowed inside a property nagivation expression.
        ///     The expression is not a property navigation expression.
        /// </exception>
        public PropertyNavigationExpression(Expression<Func<TEntity, TProperty>> propertyNavigationLambda, ParameterExpression parameterExpression)
            : this(propertyNavigationLambda.Body, parameterExpression)
        {
        }

        /// <exception cref="ArgumentException">
        ///     The property navigation expression's target property type does not match the property type.
        ///     The parameter expression's type does not match the entity type.
        ///     Only conversion unary expressions are allowed inside a property nagivation expression.
        ///     The expression is not a property navigation expression.
        /// </exception>
        public PropertyNavigationExpression(Expression expression, ParameterExpression parameterExpression)
        {
            if (expression.Type != typeof(TProperty))
            {
                throw new ArgumentException($"The property navigation expression's target property type ({expression.Type}) does not match the property type ({typeof(TProperty)}).", nameof(expression));
            }
            if (parameterExpression.Type != typeof(TEntity))
            {
                throw new ArgumentException($"The parameter expression's type ({parameterExpression.Type}) does not match the entity type ({typeof(TEntity)}).", nameof(parameterExpression));
            }

            Expression propertyNavigationExpression = expression;

            Stack<Type> conversionTypes = new();
            while (propertyNavigationExpression is UnaryExpression unaryExpression)
            {
                if (unaryExpression.NodeType != ExpressionType.Convert)
                {
                    throw new ArgumentException($"Only conversion unary expressions are allowed inside a property nagivation expression. ({unaryExpression})", nameof(expression));
                }
                conversionTypes.Push(unaryExpression.Type);
                propertyNavigationExpression = unaryExpression.Operand;
            }

            Stack<PropertyInfo> propertyInfosStack = new();
            while (propertyNavigationExpression is not ParameterExpression)
            {
                if (propertyNavigationExpression is not MemberExpression memberExpression ||
                    memberExpression.Member is not PropertyInfo propertyInfo ||
                    memberExpression.Expression == null
                )
                {
                    throw new ArgumentException($"The expression ({expression}) is not a property navigation expression.");
                }
                propertyInfosStack.Push(propertyInfo);
                propertyNavigationExpression = memberExpression.Expression;
            }
            if (propertyInfosStack.Count < 1)
            {
                throw new ArgumentException($"The expression ({expression}) is not a property navigation expression.");
            }

            propertyNavigationExpression = Expression.Property(parameterExpression, propertyInfosStack.Pop());
            while (propertyInfosStack.Count > 0)
            {
                propertyNavigationExpression = Expression.Property(propertyNavigationExpression, propertyInfosStack.Pop());
            }
            while (conversionTypes.Count > 0)
            {
                propertyNavigationExpression = Expression.Convert(propertyNavigationExpression, conversionTypes.Pop());
            }
            Expression = propertyNavigationExpression;
        }
    }
}
