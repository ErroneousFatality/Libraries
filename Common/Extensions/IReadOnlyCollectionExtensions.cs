using System.Collections.Immutable;

namespace AndrejKrizan.Common.Extensions
{
    public static class IReadOnlyCollectionExtensions
    {
        #region Transformations
        public static ImmutableArray<T> ToImmutableArray<T>(this IReadOnlyCollection<T> collection)
            => collection.ToImmutableArray(collection.Count);

        public static HashSet<T> GetDuplicateSet<T>(this IReadOnlyCollection<T> collection)
            => collection.GetDuplicateSet(collection.Count);
        public static bool HasDuplicates<T>(this IReadOnlyCollection<T> collection)
            => collection.GetDuplicateSet().Count > 0;

        public static HashSet<V> GetDuplicateValueSet<T, V>(this IReadOnlyCollection<T> collection, Func<T, V> valueSelector)
            => collection
                .Select(valueSelector)
                .GetDuplicateSet(collection.Count);
        public static bool HasDuplicateValues<T, V>(this IReadOnlyCollection<T> collection, Func<T, V> valueSelector)
            => collection.GetDuplicateValueSet(valueSelector).Count > 0;

        #endregion Transformations

        #region Checks

        public static bool SetEquals<T>(this IReadOnlyCollection<T> collection, IEnumerable<T> other)
            => collection.SetEquals(collection.Count, other);


        public static bool ContentEquals<T>(this IReadOnlyCollection<T> collection, IEnumerable<T> target)
            where T : notnull
            => collection.ContentEquals(target, collection.Count);

        public static bool ContentEquals<T>(this IReadOnlyCollection<T> collection, IReadOnlyCollection<T> target)
            where T : notnull
            => collection.Count == target.Count && collection.ContentEquals(target, collection.Count);


        public static IReadOnlyCollection<T> AssertIsDistinct<T>(this IReadOnlyCollection<T> collection,
            Func<IReadOnlySet<T>, string> exceptionMessageFunc,
            string parameterName
        )
        {
            collection.AssertIsDistinct(collection.Count, exceptionMessageFunc, parameterName);
            return collection;
        }

        public static IReadOnlyCollection<T> AssertIsDistinct<T, TProperty>(this IReadOnlyCollection<T> collection,
            Func<T, TProperty> selector,
            Func<IReadOnlySet<TProperty>, string> exceptionMessageFunc,
            string parameterName
        )
        {
            collection
                .Select(selector)
                .AssertIsDistinct(collection.Count, exceptionMessageFunc, parameterName);
            return collection;
        }
        #endregion Checks

        #region Conversions
        public static ImmutableArray<TResult> Convert<T, TResult>(this IReadOnlyCollection<T> collection, Func<T, TResult> converter)
            => collection
                .Select(converter)
                .ToImmutableArray(collection.Count);

        public static ImmutableArray<TResult> Convert<T, TResult>(this IReadOnlyCollection<T> collection, Func<T, int, TResult> converter)
            => collection
                .Select(converter)
                .ToImmutableArray(collection.Count);

        public static ImmutableArray<TResult> ConvertAndOrderBy<T, TResult>(this IReadOnlyCollection<T> collection, Func<T, TResult> converter)
            => collection
                .Select(converter)
                .Order()
                .ToImmutableArray(collection.Count);

        public static ImmutableArray<TResult> ConvertAndOrderBy<T, TResult, TProperty>(this IReadOnlyCollection<T> collection, Func<T, TResult> converter, Func<TResult, TProperty> orderBy)
            => collection
                .Select(converter)
                .OrderBy(orderBy)
                .ToImmutableArray(collection.Count);

        public static ImmutableArray<TResult> ConvertAndOrderBy<T, TResult, TProperty>(this IReadOnlyCollection<T> collection, Func<T, TResult> converter, Func<T, TProperty> orderBy)
            => collection
                .OrderBy(orderBy)
                .Select(converter)
                .ToImmutableArray(collection.Count);

        public static ImmutableArray<TResult> ConvertAndOrderByDescending<T, TResult, TProperty>(this IReadOnlyCollection<T> collection, Func<T, TResult> converter)
            => collection
                .Select(converter)
                .OrderDescending()
                .ToImmutableArray(collection.Count);

        public static ImmutableArray<TResult> ConvertAndOrderByDescending<T, TResult, TProperty>(this IReadOnlyCollection<T> collection, Func<T, TResult> converter, Func<TResult, TProperty> orderBy)
            => collection
                .Select(converter)
                .OrderByDescending(orderBy)
                .ToImmutableArray(collection.Count);

        public static async Task<ImmutableArray<TResult>> ConvertAsync<T, TResult>(this IReadOnlyCollection<T> items,
            Func<T, Task<TResult>> asyncConverter, bool concurrently = false,
            CancellationToken cancellationToken = default
        )
            => await items.ConvertAsync(items.Count, asyncConverter, concurrently, cancellationToken);

        public static async Task<ImmutableArray<TResult>> ConvertAsync<T, TResult>(this IReadOnlyCollection<T> items,
            Func<T, CancellationToken, Task<TResult>> asyncConverter, bool concurrently = false,
            CancellationToken cancellationToken = default
        )
            => await items.ConvertAsync(items.Count, asyncConverter, concurrently, cancellationToken);


        public static T[] ToArray<T>(this IReadOnlyCollection<T> collection)
            => collection.ToArray(collection.Count);

        public static TResult[] ToArray<T, TResult>(this IReadOnlyCollection<T> collection, Func<T, TResult> selector)
            => collection
                .Select(selector)
                .ToArray(collection.Count);

        public static TResult[] ToArray<T, TResult>(this IReadOnlyCollection<T> collection, Func<T, int, TResult> selector)
            => collection
                .Select(selector)
                .ToArray(collection.Count);
        #endregion Conversions
    }
}
