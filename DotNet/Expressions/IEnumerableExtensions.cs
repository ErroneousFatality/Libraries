using System.Linq.Expressions;

using AndrejKrizan.DotNet.Strings;

namespace AndrejKrizan.DotNet.Expressions;
public static class IEnumerableExtensions
{
    #region ToPredicate
    public static Expression<Func<T, bool>> ToPredicateLambda<TData, T>(this IEnumerable<TData> dataSource, Func<TData, Expression<Func<T, bool>>> predicateBuilder)
    {
        if (!dataSource.Any())
        {
            return (item) => false;
        }
        ParameterExpression parameter = Expression.Parameter(typeof(T), typeof(T).Name.ToLowercasedFirstCharacterInvariant());
        Expression predicateExpression = dataSource
            .Select(data => predicateBuilder(data).ReplaceParameter(parameter).Body)
            .Aggregate(Expression.OrElse)!;
        Expression<Func<T, bool>> predicateLambda = Expression.Lambda<Func<T, bool>>(predicateExpression, parameter);
        return predicateLambda;
    }

    public static Func<T, bool> ToPredicateFunc<TData, T>(this IEnumerable<TData> dataSource, Func<TData, Expression<Func<T, bool>>> predicateBuilder)
    {
        Expression<Func<T, bool>> predicateLambda = dataSource.ToPredicateLambda(predicateBuilder);
        Func<T, bool> predicateFunc = predicateLambda.Compile();
        return predicateFunc;
    }
    #endregion

    public static IEnumerable<T> WhereAny<T, TData>(this IEnumerable<T> source, IEnumerable<TData> dataSource, Func<TData, Expression<Func<T, bool>>> predicateBuilder)
    {
        if (dataSource.Any())
        {
            Func<T, bool> predicate = dataSource.ToPredicateFunc(predicateBuilder);
            source = source.Where(predicate);
        }
        return source;
    }
}
