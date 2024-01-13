using System.Linq.Expressions;

namespace AndrejKrizan.DotNet.Lambdas;
public static class LambdaUtils
{
    public static Expression<Func<TSource, TResult>> Extend<TSource, T, TResult>(
        Expression<Func<TSource, T>> source,
        Expression<Func<T, TResult>> extension
    )
        => source.Extend(extension);

    #region Explode
    public static IEnumerable<Expression<Func<TSource, TResult>>> Explode<TSource, T, TResult>(
        Expression<Func<TSource, T>> source,
        IEnumerable<Expression<Func<T, TResult>>> extensions
    )
        => source.Explode(extensions);

    public static IEnumerable<Expression<Func<TSource, object>>> Explode<TSource, T>(
        Expression<Func<TSource, T>> source,
        IEnumerable<Expression<Func<T, object>>> extensions
    )
        => source.Explode(extensions);
    #endregion Explode
}
