using System.Collections.Immutable;
using System.Linq.Expressions;

using AndrejKrizan.DotNet.Collections;

namespace AndrejKrizan.DotNet.Lambdas;
public static class IEnumerableExtensions
{
    #region Any
    public static Expression<Func<T, bool>> ToAnyLambda<T, TArgument>(this IEnumerable<TArgument> arguments,
        Func<TArgument, Expression<Func<T, bool>>> predicateBuilder
    )
    {
        if (!arguments.Any())
        {
            return _ => true;
        }
        ImmutableArray<Expression<Func<T, bool>>> predicates = arguments.Convert(argument => predicateBuilder(argument));
        ParameterExpression parameter = predicates[0].Parameters[0];
        Expression anyExpression = predicates
            .Select(predicate => predicate.ReplaceParameter(parameter).Body)
            .Aggregate(Expression.OrElse);
        Expression<Func<T, bool>> anyLambda = Expression.Lambda<Func<T, bool>>(anyExpression, parameter);
        return anyLambda;
    }

    public static Func<T, bool> ToAnyFunc<T, TArgument>(this IEnumerable<TArgument> arguments,
        Func<TArgument, Expression<Func<T, bool>>> predicateBuilder
    )
    {
        Expression<Func<T, bool>> anyLambda = arguments.ToAnyLambda(predicateBuilder);
        Func<T, bool> anyFunc = anyLambda.Compile();
        return anyFunc;
    }

    public static IEnumerable<T> WhereAny<T, TArgument>(this IEnumerable<T> source,
        IEnumerable<TArgument>? arguments,
        Func<TArgument, Expression<Func<T, bool>>> predicateBuilder
    )
    {
        if (arguments == null || !arguments.Any())
        {
            return source;
        }
        Func<T, bool> any = arguments.ToAnyFunc(predicateBuilder);
        IEnumerable<T> whereAny = source.Where(any);
        return whereAny;
    }
    #endregion

    #region Every
    public static Expression<Func<T, bool>> ToEveryLambda<T, TArgument>(this IEnumerable<TArgument> arguments,
        Func<TArgument, Expression<Func<T, bool>>> predicateBuilder
    )
    {
        if (!arguments.Any())
        {
            return _ => true;
        }
        ImmutableArray<Expression<Func<T, bool>>> predicates = arguments.Convert(argument => predicateBuilder(argument));
        ParameterExpression parameter = predicates[0].Parameters[0];
        Expression everyExpression = predicates
            .Select(predicate => predicate.ReplaceParameter(parameter).Body)
            .Aggregate(Expression.AndAlso);
        Expression<Func<T, bool>> everyLambda = Expression.Lambda<Func<T, bool>>(everyExpression, parameter);
        return everyLambda;
    }

    public static Func<T, bool> ToEveryFunc<T, TArgument>(this IEnumerable<TArgument> arguments,
        Func<TArgument, Expression<Func<T, bool>>> predicateBuilder
    )
    {
        Expression<Func<T, bool>> everyLambda = arguments.ToEveryLambda(predicateBuilder);
        Func<T, bool> everyFunc = everyLambda.Compile();
        return everyFunc;
    }

    public static IEnumerable<T> WhereEvery<T, TArgument>(this IEnumerable<T> source,
        IEnumerable<TArgument>? arguments,
        Func<TArgument, Expression<Func<T, bool>>> predicateBuilder
    )
    {
        if (arguments == null || !arguments.Any())
        {
            return source;
        }
        Func<T, bool> every = arguments.ToEveryFunc(predicateBuilder);
        IEnumerable<T> whereEvery = source.Where(every);
        return whereEvery;
    }
    #endregion
}
