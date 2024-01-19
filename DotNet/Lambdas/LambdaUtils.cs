using System.Linq.Expressions;

namespace AndrejKrizan.DotNet.Lambdas;
public static class LambdaUtils
{
    #region Extend
    public static Expression<Func<TSource, TResult>> Extend<TSource, T, TResult>(
        Expression<Func<TSource, T>> source,
        Expression<Func<T, TResult>> extension
    )
        => source.Extend(extension);

    public static LambdaExpression Extend(LambdaExpression source, LambdaExpression extension)
        => source.Extend(extension);

    #endregion

    #region Explode
    public static IEnumerable<Expression<Func<TSource, TResult>>> Explode<TSource, T, TResult>(
        Expression<Func<TSource, T>> source,
        IEnumerable<Expression<Func<T, TResult>>> extensions
    )
        => source.Explode(extensions);

    public static IEnumerable<LambdaExpression> Explode(LambdaExpression source, IEnumerable<LambdaExpression> extensions)
        => source.Explode(extensions);
    #endregion Explode
}
