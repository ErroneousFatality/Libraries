using System.Collections.Immutable;
using System.Linq.Expressions;

using AndrejKrizan.DotNet.Extensions;
using AndrejKrizan.DotNet.ValueObjects.Ranges;

namespace AndrejKrizan.DotNet.Extensions;

public static class IEnumerableExtensions
{
    #region StringJoin

    public static string StringJoin(this IEnumerable<string?> source, string? separator = ", ", bool quote = false)
    {
        if (quote)
        {
            source = source.Select(str => str.Quote());
        }
        return string.Join(separator, source);
    }

    public static string StringJoin(this IEnumerable<string?> source, char separator, bool quote = false)
    {
        if (quote)
        {
            source = source.Select(str => str.Quote());
        }
        return string.Join(separator, source);
    }


    public static string StringJoin<T>(this IEnumerable<T?> source, string? separator = ", ", bool quote = false)
        => source.Select(str => str?.ToString()).StringJoin(separator, quote);

    public static string StringJoin<T>(this IEnumerable<T?> source, char separator, bool quote = false)
        => source.Select(str => str?.ToString()).StringJoin(separator, quote);
    #endregion

    public static ImmutableArray<T> ToImmutableArray<T>(this IEnumerable<T> source, int count)
    {
        ImmutableArray<T>.Builder arrayBuilder = ImmutableArray.CreateBuilder<T>(count);
        foreach (T item in source)
        {
            arrayBuilder.Add(item);
        }
        ImmutableArray<T> array = arrayBuilder.MoveToImmutable();
        return array;
    }

    #region Convert
    public static ImmutableArray<TResult> Convert<T, TResult>(this IEnumerable<T> items, Func<T, TResult> selector)
        => items.Select(selector).ToImmutableArray();

    public static ImmutableArray<TResult> Convert<T, TResult>(this IEnumerable<T> items, Func<T, int, TResult> selector)
        => items.Select(selector).ToImmutableArray();
    #endregion

    public static async Task<ImmutableArray<TResult>> ConvertConcurrentlyAsync<T, TResult>(this IEnumerable<T> items,
        Func<T, CancellationToken, Task<TResult>> asyncSelector,
        CancellationToken cancellationToken = default
    )
    {
        IEnumerable<Task<TResult>> conversionTasks = items.Select(item => asyncSelector(item, cancellationToken));
        TResult[] _results = await Task.WhenAll(conversionTasks);
        ImmutableArray<TResult> results = _results.AsImmutableArray();
        return results;
    }

    #region ConvertSequentiallyAsync
    public static Task<ImmutableArray<TResult>> ConvertSequentiallyAsync<T, TResult>(this IEnumerable<T> source,
        Func<T, CancellationToken, Task<TResult>> asyncSelector,
        CancellationToken cancellationToken = default
    )
        => source.ConvertSequentiallyAsync(asyncSelector, source.Count(), cancellationToken);

    public static async Task<ImmutableArray<TResult>> ConvertSequentiallyAsync<T, TResult>(this IEnumerable<T> source,
        Func<T, CancellationToken, Task<TResult>> asyncSelector, int size,
        CancellationToken cancellationToken = default
    )
    {
        ImmutableArray<TResult>.Builder resultsBuilder = ImmutableArray.CreateBuilder<TResult>(size);
        foreach (T item in source)
        {
            TResult result = await asyncSelector(item, cancellationToken);
            resultsBuilder.Add(result);
        }
        ImmutableArray<TResult> results = resultsBuilder.MoveToImmutable();
        return results;
    }
    #endregion

    #region Average
    public static ulong Average(this IEnumerable<ulong> source)
        => decimal.ToUInt64(source.Select(System.Convert.ToDecimal).Average());
    public static ulong Average<T>(this IEnumerable<T> source, Func<T, ulong> selector)
        => source.Select(selector).Average();

    public static long AverageLong(this IEnumerable<long> source)
        => decimal.ToInt64(source.Select(System.Convert.ToDecimal).Average());
    public static long AverageLong<T>(this IEnumerable<T> source, Func<T, long> selector)
        => source.Select(selector).AverageLong();
    #endregion Average

    #region HashCode
    public static int GetContentHashCode<T>(this IEnumerable<T> set)
    {
        HashCode hashCode = new();
        foreach (T item in set)
        {
            hashCode.Add(item);
        }
        return hashCode.ToHashCode();
    }

    public static int GetSequenceHashCode<T>(this IEnumerable<T> items)
        => items.Aggregate(19, (hashCode, item) => hashCode * 31 + item?.GetHashCode() ?? 0);
    #endregion

    #region Duplicates
    public static HashSet<T> GetDuplicateSet<T>(this IEnumerable<T> source)
        => source.GetDuplicateSet(EqualityComparer<T>.Default);

    public static HashSet<T> GetDuplicateSet<T>(this IEnumerable<T> source, IEqualityComparer<T> equalityComparer)
    {
        bool hasCount = source.TryGetNonEnumeratedCount(out int count);
        HashSet<T> set = hasCount
            ? new(count, equalityComparer)
            : new(equalityComparer);
        HashSet<T> duplicateSet = hasCount
            ? new(count / 2, equalityComparer)
            : new(equalityComparer);
        foreach (T item in source)
        {
            if (!set.Add(item))
            {
                duplicateSet.Add(item);
            }
        }
        return duplicateSet;
    }


    public static bool HasDuplicates<T>(this IEnumerable<T> source)
        => source.HasDuplicates(EqualityComparer<T>.Default);

    public static bool HasDuplicates<T>(this IEnumerable<T> source, IEqualityComparer<T> equalityComparer)
        => source.GetDuplicateSet(equalityComparer).Count > 0;


    /// <exception cref="ArgumentException"></exception>
    public static void RequireIsDistinct<T>(this IEnumerable<T> source,
        Func<IReadOnlySet<T>, string> errorMessageFunc, string parameterName
    )
        => source.RequireIsDistinct(errorMessageFunc, parameterName, EqualityComparer<T>.Default);

    /// <exception cref="ArgumentException"></exception>
    public static void RequireIsDistinct<T>(this IEnumerable<T> source,
        Func<IReadOnlySet<T>, string> errorMessageFunc, string parameterName,
        IEqualityComparer<T> equalityComparer
    )
        => source.AssertIsDistinct(duplicates => new ArgumentException(errorMessageFunc(duplicates), parameterName), equalityComparer);


    /// <exception cref="InvalidOperationException"></exception>
    public static void EnsureIsDistinct<T>(this IEnumerable<T> source, Func<IReadOnlySet<T>, string> errorMessageFunc)
        => source.EnsureIsDistinct(errorMessageFunc, EqualityComparer<T>.Default);

    /// <exception cref="InvalidOperationException"></exception>
    public static void EnsureIsDistinct<T>(this IEnumerable<T> source,
        Func<IReadOnlySet<T>, string> errorMessageFunc, IEqualityComparer<T> equalityComparer
    )
        => source.AssertIsDistinct(duplicates => new InvalidOperationException(errorMessageFunc(duplicates)), equalityComparer);

    /// <exception cref="Exception"></exception>
    public static void AssertIsDistinct<T>(this IEnumerable<T> source, Func<IReadOnlySet<T>, Exception> errorFunc)
        => source.AssertIsDistinct(errorFunc, EqualityComparer<T>.Default);

    /// <exception cref="Exception"></exception>
    public static void AssertIsDistinct<T>(this IEnumerable<T> source,
        Func<IReadOnlySet<T>, Exception> errorFunc, IEqualityComparer<T> equalityComparer
    )
    {
        IReadOnlySet<T> duplicates = source.GetDuplicateSet(equalityComparer);
        if (duplicates.Count > 0)
        {
            Exception error = errorFunc(duplicates);
            throw error;
        }
    }

    #endregion

    #region Equals
    public static bool SetEquals<T>(this IEnumerable<T> source, IEnumerable<T> other, IEqualityComparer<T> comparer)
        => new HashSet<T>(source, comparer).SetEquals(other);

    public static bool SetEquals<T>(this IEnumerable<T> source, IEnumerable<T> other)
        => SetEquals(source, other, EqualityComparer<T>.Default);


    public static bool ContentEquals<T>(this IEnumerable<T> source, IEnumerable<T> target, IEqualityComparer<T> comparer)
        where T : notnull
    {
        bool hasCount = source.TryGetNonEnumeratedCount(out int count);
        if (hasCount && target.TryGetNonEnumeratedCount(out int targetCount) && count != targetCount)
        {
            return false;
        }
        Dictionary<T, int> countByItemDictionary = hasCount ? new(count, comparer) : new(comparer);
        foreach (T item in source)
        {
            countByItemDictionary[item] = countByItemDictionary.TryGetValue(item, out int itemCount)
                ? itemCount + 1
                : 1;
        }
        foreach (T item in target)
        {
            if (!countByItemDictionary.TryGetValue(item, out int itemCount) || itemCount < 1)
            {
                return false;
            }
            countByItemDictionary[item] = itemCount - 1;
        }
        return true;
    }

    public static bool ContentEquals<T>(this IEnumerable<T> source, IEnumerable<T> target)
        where T : notnull
        => ContentEquals(source, target, EqualityComparer<T>.Default);
    #endregion

    #region MinBy
    public static T MinBy<T, TValue>(this IEnumerable<T> source, Func<T, TValue> valueSelector, out TValue minValue, IComparer<TValue> comparer)
    {
        T minItem = source.First();
        minValue = valueSelector(minItem);

        foreach (T item in source.Skip(1))
        {
            TValue value = valueSelector(item);
            if (comparer.Compare(value, minValue) < 0)
            {
                minItem = item;
                minValue = value;
            }
        }
        return minItem;
    }
    public static T MinBy<T, TValue>(this IEnumerable<T> source, Func<T, TValue> valueSelector, out TValue minValue)
        => source.MinBy(valueSelector, out minValue, Comparer<TValue>.Default);

    public static T? MinByOrDefault<T, TValue>(this IEnumerable<T> source, Func<T, TValue> valueSelector, out TValue? minValue, IComparer<TValue> comparer)
    {
        if (!source.Any())
        {
            minValue = default;
            return default;
        }
        return source.MinBy(valueSelector, out minValue, comparer);
    }
    public static T? MinByOrDefault<T, TValue>(this IEnumerable<T> source, Func<T, TValue> valueSelector, out TValue? minValue)
        => source.MinByOrDefault(valueSelector, out minValue, Comparer<TValue>.Default);
    #endregion

    #region MaxBy
    public static T MaxBy<T, TValue>(this IEnumerable<T> source, Func<T, TValue> valueSelector, out TValue maxValue, IComparer<TValue> comparer)
    {
        T maxItem = source.First();
        maxValue = valueSelector(maxItem);

        foreach (T item in source.Skip(1))
        {
            TValue value = valueSelector(item);
            if (comparer.Compare(value, maxValue) > 0)
            {
                maxItem = item;
                maxValue = value;
            }
        }
        return maxItem;
    }
    public static T MaxBy<T, TValue>(this IEnumerable<T> source, Func<T, TValue> valueSelector, out TValue maxValue)
        => source.MaxBy(valueSelector, out maxValue, Comparer<TValue>.Default);

    public static T? MaxByOrDefault<T, TValue>(this IEnumerable<T> source, Func<T, TValue> valueSelector, out TValue? maxValue, IComparer<TValue> comparer)
    {
        if (!source.Any())
        {
            maxValue = default;
            return default;
        }
        return source.MaxBy(valueSelector, out maxValue, comparer);
    }
    public static T? MaxByOrDefault<T, TValue>(this IEnumerable<T> source, Func<T, TValue> valueSelector, out TValue? maxValue)
        => source.MaxByOrDefault(valueSelector, out maxValue, Comparer<TValue>.Default);
    #endregion

    #region IndexOf
    public static int IndexOf<T>(this IEnumerable<T> source, T item, IComparer<T> comparer)
    {
        int index = 0;
        foreach (T _item in source)
        {
            if (comparer.Compare(_item, item) == 0)
            {
                return index;
            }
            index++;
        }
        return -1;
    }
    public static int IndexOf<T>(this IEnumerable<T> source, T item)
        => source.IndexOf(item, Comparer<T>.Default);
    #endregion

    #region Range
    public static Range<T> GetRange<T>(this IEnumerable<T> source, IComparer<T> comparer)
        where T : struct
    {
        T min = source.First();
        T max = min;

        foreach (T item in source.Skip(1))
        {
            if (comparer.Compare(item, min) < 0)
            {
                min = item;
            }
            else if (comparer.Compare(item, max) > 0)
            {
                max = item;
            }
        }
        return new Range<T>(from: min, to: max, validate: false);
    }
    public static Range<T> GetRange<T>(this IEnumerable<T> source)
        where T : struct
        => source.GetRange(Comparer<T>.Default);
    #endregion

    public static ImmutableArray<ImmutableArray<T>> Transpose<T, TRow>(this IEnumerable<TRow> columns)
        where TRow : IEnumerable<T>
    {
        int width = columns.Count();
        int height = columns.FirstOrDefault()?.Count() ?? 0;
        if (height == 0 || width == 0)
        {
            return ImmutableArray<ImmutableArray<T>>.Empty;
        }
        if (columns.Skip(1).Any(column => column.Count() != height))
        {
            throw new ArgumentException("All the columns must be the same size.", nameof(columns));
        }
        ImmutableArray<T>.Builder[] rowBuilders = new ImmutableArray<T>.Builder[height].Fill(_ => ImmutableArray.CreateBuilder<T>(width));
        foreach (IEnumerable<T> column in columns)
        {
            int rowIndex = 0;
            foreach (T item in column)
            {
                rowBuilders[rowIndex++].Add(item);
            }
        }
        ImmutableArray<ImmutableArray<T>> matrix = rowBuilders.Convert(rowBuilder => rowBuilder.MoveToImmutable());
        return matrix;
    }

    public static IEnumerable<T> WhereAny<T, TData>(this IEnumerable<T> source, IEnumerable<TData> dataSource, Func<TData, Expression<Func<T, bool>>> predicateBuilder)
    {
        if (dataSource.Any())
        {
            Func<T, bool> predicate = dataSource.ToPredicateFunc(predicateBuilder);
            source = source.Where(predicate);
        }
        return source;
    }

    #region ToPredicate
    public static Expression<Func<T, bool>> ToPredicateLambda<TData, T>(this IEnumerable<TData> dataSource, Func<TData, Expression<Func<T, bool>>> predicateBuilder)
    {
        if (!dataSource.Any())
        {
            return (T item) => true;
        }
        string parameterName = typeof(T).Name.ToLowercasedFirstCharacterInvariant();
        ParameterExpression parameterExpression = Expression.Parameter(typeof(T), parameterName);

        IEnumerable<Expression> predicateExpressions = dataSource.Select(data
            => predicateBuilder(data)
                .ReplaceParameters(parameterExpression)
                .Body
        );
        Expression predicateExpression = predicateExpressions.Aggregate(Expression.OrElse)!;
        Expression<Func<T, bool>> predicateLambda = Expression.Lambda<Func<T, bool>>(predicateExpression, parameterExpression);
        return predicateLambda;
    }

    public static Func<T, bool> ToPredicateFunc<TData, T>(this IEnumerable<TData> dataSource, Func<TData, Expression<Func<T, bool>>> predicateBuilder)
    {
        Expression<Func<T, bool>> predicateLambda = dataSource.ToPredicateLambda(predicateBuilder);
        Func<T, bool> predicateFunc = predicateLambda.Compile();
        return predicateFunc;
    }
    #endregion

    #region ToDictionary
    public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source)
        where TKey : notnull
        => new Dictionary<TKey, TValue>(source);

    public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source, IEqualityComparer<TKey>? equalityComparer)
        where TKey : notnull
        => new Dictionary<TKey, TValue>(source, equalityComparer);
    #endregion
}
