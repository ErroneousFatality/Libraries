using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace AndrejKrizan.DotNet.Expressions;
public static class LambdaExtensions
{
    #region Parameter replacement
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
    #endregion
}
