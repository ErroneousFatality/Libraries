using System.Collections.ObjectModel;
using System.Linq.Expressions;

using AndrejKrizan.DotNet.Expressions;

namespace AndrejKrizan.DotNet.Lambdas;
public static class LambdaExtensions
{
    #region Replace parameters
    public static TLambda ReplaceParameter<TLambda>(this TLambda source, ParameterExpression parameter)
       where TLambda : LambdaExpression
    {
        ReadOnlyCollection<ParameterExpression> sourceParameters = source.Parameters;
        if (sourceParameters.Count != 1)
        {
            throw new ArgumentException("The source lambda must have exactly one parameter.", nameof(source));
        }
        ParameterExpression sourceParameter = sourceParameters[0];
        if (parameter.Type != sourceParameter.Type)
        {
            throw new ArgumentException($"The new parameter ({parameter.Type} {parameter.Name}) must have the same type as the old parameter ({sourceParameter.Type} {sourceParameter.Name}).", nameof(parameter));
        }
        TLambda lambda = (TLambda)new ParameterReplacer(sourceParameter, parameter).Visit(source);
        return lambda;
    }

    public static TLambda ReplaceParameters<TLambda>(this TLambda source, params ParameterExpression[] parameters)
        where TLambda : LambdaExpression
    {
        ReadOnlyCollection<ParameterExpression> sourceParameters = source.Parameters;
        if (parameters.Length > sourceParameters.Count)
        {
            throw new ArgumentException($"There are more provided parameters ({parameters.Length}) than there are source lambda parameters ({sourceParameters.Count}).", nameof(parameters));
        }
        Dictionary<ParameterExpression, Expression> parameterMappings = sourceParameters
            .Zip(parameters, (_old, _new) =>
            {
                if (_new.Type != _old.Type)
                {
                    throw new ArgumentException($"The new parameter ({_new.Type} {_new.Name}) must have the same type as the old parameter ({_old.Type} {_old.Name}).", nameof(parameters));
                }
                return new KeyValuePair<ParameterExpression, Expression>(_old, _new);
            })
            .ToDictionary();
        TLambda lambda = (TLambda)new ParameterReplacer(parameterMappings).Visit(source);
        return lambda;
    }
    #endregion

    #region Extend
    public static LambdaExpression Extend(this LambdaExpression source,
        LambdaExpression extension
    )
    {
        if (extension.Parameters.Count != 1)
        {
            throw new ArgumentException("The extension lamdba must have exactly one parameter.", nameof(extension));
        }
        ParameterExpression extensionParameter = extension.Parameters[0];
        if (!source.ReturnType.IsAssignableTo(extensionParameter.Type))
        {
            throw new ArgumentException($"The source lambda's return type ({source.ReturnType}) must be assignable to the extension lambda's parameter type ({extensionParameter.Type}).", nameof(extension));
        }
        Expression expression = extension.Body.ReplaceParameter(extensionParameter, source.Body);
        LambdaExpression lambda = Expression.Lambda(expression, source.Parameters);
        return lambda;
    }

    public static Expression<Func<TSource, TResult>> Extend<TSource, T, TResult>(this Expression<Func<TSource, T>> source,
        Expression<Func<T, TResult>> extension
    )
        => (Expression<Func<TSource, TResult>>)source.Extend((LambdaExpression)extension);
    #endregion

    #region Explode
    public static IEnumerable<LambdaExpression> Explode(this LambdaExpression source,
        IEnumerable<LambdaExpression> extensions
    )
        => extensions.Select(source.Extend);

    public static IEnumerable<Expression<Func<TSource, TResult>>> Explode<TSource, T, TResult>(this Expression<Func<TSource, T>> source,
        IEnumerable<Expression<Func<T, TResult>>> extensions
    )
        => extensions.Select(extension => source.Extend(extension));

    public static IEnumerable<Expression<Func<TSource, object>>> Explode<TSource, T>(this Expression<Func<TSource, T>> source,
        IEnumerable<Expression<Func<T, object>>> extensions
    )
        => extensions.Select(extension => source.Extend(extension));
    #endregion
}
