using System.Collections.Immutable;

using AndrejKrizan.Common.Extensions;
using AndrejKrizan.Common.ValueObjects.Ranges;

namespace AndrejKrizan.Common.Extensions
{
    public static class IEnumerableExtensions
    {
        #region Transformations

        /// <summary>Optimized version.</summary>
        public static T[] ToArray<T>(this IEnumerable<T> sequence, int count)
        {
            T[] array = new T[count];
            int i = 0;
            foreach (T item in sequence)
            {
                array[i++] = item;
            }
            return array;
        }

        /// <summary>Optimized version.</summary>
        public static ImmutableArray<T> ToImmutableArray<T>(this IEnumerable<T> enumerable, int count)
        {
            ImmutableArray<T>.Builder arrayBuilder = ImmutableArray.CreateBuilder<T>(count);
            foreach (T item in enumerable)
            {
                arrayBuilder.Add(item);
            }
            ImmutableArray<T> array = arrayBuilder.MoveToImmutable();
            return array;
        }

        /// <summary>Optimized version.</summary>
        public static HashSet<T> GetDuplicateSet<T>(this IEnumerable<T> enumerable, int count)
        {
            HashSet<T> set = new(count);
            HashSet<T> duplicateSet = new(count / 2);
            foreach (T item in enumerable)
            {
                if (!set.Add(item))
                {
                    duplicateSet.Add(item);
                }
            }
            return duplicateSet;
        }
        public static HashSet<T> GetDuplicateSet<T>(this IEnumerable<T> enumerable)
            => enumerable.GetDuplicateSet(enumerable.Count());
        public static bool HasDuplicates<T>(this IEnumerable<T> enumerable)
            => enumerable.GetDuplicateSet().Count > 0;

        public static string StringJoin<T>(this IEnumerable<T> enumerable, string? separator = ", ", bool quote = false)
            where T : notnull
        {
            IEnumerable<string?> strings = quote
                ? enumerable.Select(x => $"\"{x}\"")
                : enumerable.Select(x => x.ToString());
            return string.Join(separator, strings);
        }

        public static string StringJoin<T>(this IEnumerable<T> enumerable, char separator, bool quote = false)
            where T : notnull
        {
            IEnumerable<string?> strings = quote
                ? enumerable.Select(x => $"\"{x}\"")
                : enumerable.Select(x => x.ToString());
            return string.Join(separator, strings);
        }

        public static Range<T> GetRange<T>(this IEnumerable<T> enumerable, IComparer<T> comparer)
            where T : struct
        {
            T min = enumerable.First();
            T max = min;

            foreach (T item in enumerable.Skip(1))
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
        public static Range<T> GetRange<T>(this IEnumerable<T> enumerable)
            where T : struct
            => enumerable.GetRange(Comparer<T>.Default);

        public static async Task<ImmutableArray<TResult>> ConvertAsync<T, TResult>(this IEnumerable<T> enumerable, int size,
            Func<T, Task<TResult>> asyncConverter, bool concurrently = false,
            CancellationToken cancellationToken = default
        )
        {
            cancellationToken.ThrowIfCancellationRequested();
            ImmutableArray<TResult> results;
            if (concurrently)
            {
                Task<TResult>[] conversionTasks = enumerable
                    .Select(item => asyncConverter(item))
                    .ToArray(size);
                TResult[] resultsArray = await Task.WhenAll(conversionTasks);
                cancellationToken.ThrowIfCancellationRequested();
                results = ImmutableArray.Create(resultsArray);
            }
            else
            {
                ImmutableArray<TResult>.Builder resultsBuilder = ImmutableArray.CreateBuilder<TResult>(size);
                foreach (T item in enumerable)
                {
                    TResult result = await asyncConverter(item);
                    cancellationToken.ThrowIfCancellationRequested();
                    resultsBuilder.Add(result);
                }
                results = resultsBuilder.MoveToImmutable();
            }
            return results;
        }

        public static async Task<ImmutableArray<TResult>> ConvertAsync<T, TResult>(this IEnumerable<T> enumerable, int size,
            Func<T, CancellationToken, Task<TResult>> asyncConverter, bool concurrently = false,
            CancellationToken cancellationToken = default
        )
        {
            ImmutableArray<TResult> results;
            if (concurrently)
            {
                Task<TResult>[] conversionTasks = enumerable
                    .Select(item => asyncConverter(item, cancellationToken))
                    .ToArray(size);
                TResult[] resultsArray = await Task.WhenAll(conversionTasks);
                results = ImmutableArray.Create(resultsArray);
            }
            else
            {
                ImmutableArray<TResult>.Builder resultsBuilder = ImmutableArray.CreateBuilder<TResult>(size);
                foreach (T item in enumerable)
                {
                    TResult result = await asyncConverter(item, cancellationToken);
                    resultsBuilder.Add(result);
                }
                results = resultsBuilder.MoveToImmutable();
            }
            return results;
        }
        public static async Task<ImmutableArray<TResult>> ConvertAsync<T, TResult>(this IEnumerable<T> enumerable,
            Func<T, CancellationToken, Task<TResult>> asyncConverter, bool concurrently = false,
            CancellationToken cancellationToken = default
        )
            => await enumerable.ConvertAsync(enumerable.Count(), asyncConverter, concurrently, cancellationToken);

        public static ImmutableArray<IReadOnlyCollection<T>> Transpose<T>(this IEnumerable<ImmutableArray<T>> columns)
            => columns.Transpose<T, ImmutableArray<T>>(height: columns.FirstOrDefault().Length, width: columns.Count());
        public static ImmutableArray<IReadOnlyCollection<T>> Transpose<T>(this IEnumerable<IEnumerable<T>> columns)
            => columns.Transpose<T, IEnumerable<T>>(height: columns.FirstOrDefault()?.Count() ?? 0, width: columns.Count());
        private static ImmutableArray<IReadOnlyCollection<T>> Transpose<T, TRow>(this IEnumerable<TRow> columns, int height, int width)
            where TRow : IEnumerable<T>
        {
            if (height == 0 || width == 0)
            {
                return ImmutableArray<IReadOnlyCollection<T>>.Empty;
            }
            if (columns.Skip(1).Any(column => column.Count() != height))
            {
                throw new ArgumentException("All the columns must be the same size.", nameof(columns));
            }
            ImmutableArray<ImmutableArray<T>.Builder> rowBuilders = Enumerable.Range(1, height)
                .Select(_ => ImmutableArray.CreateBuilder<T>(width))
                .ToImmutableArray(height);
            foreach (IEnumerable<T> column in columns)
            {
                int rowIndex = 0;
                foreach (T item in column)
                {
                    rowBuilders[rowIndex++].Add(item);
                }
            }
            ImmutableArray<IReadOnlyCollection<T>> matrix = rowBuilders.Convert(rowBuilder => (IReadOnlyCollection<T>)rowBuilder.MoveToImmutable());
            return matrix;
        }

        #region Average
        public static ulong Average(this IEnumerable<ulong> enumerable)
            => decimal.ToUInt64(enumerable.Select(Convert.ToDecimal).Average());
        public static ulong Average<T>(this IEnumerable<T> enumerable, Func<T, ulong> selector)
            => enumerable.Select(selector).Average();

        public static long AverageLong(this IEnumerable<long> enumerable)
            => decimal.ToInt64(enumerable.Select(Convert.ToDecimal).Average());
        public static long AverageLong<T>(this IEnumerable<T> enumerable, Func<T, long> selector)
            => enumerable.Select(selector).AverageLong();
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

        #endregion Transformations

        #region Checks
        public static bool SetEquals<T>(this IEnumerable<T> enumerable, IEnumerable<T> other)
            => new HashSet<T>(enumerable).SetEquals(other);

        /// <summary>Optimized version.</summary>
        public static bool SetEquals<T>(this IEnumerable<T> enumerable, int count, IEnumerable<T> other)
        {
            HashSet<T> set = new(count);
            foreach (T item in enumerable)
            {
                set.Add(item);
            }
            return set.SetEquals(other);
        }


        public static bool ContentEquals<T>(this IEnumerable<T> enumerable, IEnumerable<T> target)
            where T : notnull
            => ContentEqualsInternal(enumerable, target, new Dictionary<T, int>());

        /// <summary>Optimized version.</summary>
        public static bool ContentEquals<T>(this IEnumerable<T> enumerable, IEnumerable<T> target, int count)
            where T : notnull
            => ContentEqualsInternal(enumerable, target, new Dictionary<T, int>(count));

        private static bool ContentEqualsInternal<T>(IEnumerable<T> enumerable, IEnumerable<T> target, Dictionary<T, int> countByItemDictionary)
            where T : notnull
        {
            foreach (T item in enumerable)
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

        public static IEnumerable<T> AssertIsDistinct<T>(this IEnumerable<T> enumerable, Func<IReadOnlySet<T>, string> exceptionMessageFunc, string parameterName)
        {
            IReadOnlySet<T> duplicates = enumerable.GetDuplicateSet();
            if (duplicates.Count > 0)
            {
                string exceptionMessage = exceptionMessageFunc(duplicates);
                throw new ArgumentException(exceptionMessage, parameterName);
            }
            return enumerable;
        }

        public static IEnumerable<T> AssertIsDistinct<T>(this IEnumerable<T> enumerable, int count, Func<IReadOnlySet<T>, string> exceptionMessageFunc, string parameterName)
        {
            IReadOnlySet<T> duplicates = enumerable.GetDuplicateSet(count);
            if (duplicates.Count > 0)
            {
                string exceptionMessage = exceptionMessageFunc(duplicates);
                throw new ArgumentException(exceptionMessage, parameterName);
            }
            return enumerable;
        }

        #endregion Checks

        #region Search
        public static T MinBy<T, TValue>(this IEnumerable<T> enumerable, Func<T, TValue> valueSelector, out TValue minValue, IComparer<TValue> comparer)
        {
            T minItem = enumerable.First();
            minValue = valueSelector(minItem);

            foreach (T item in enumerable.Skip(1))
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
        public static T MinBy<T, TValue>(this IEnumerable<T> enumerable, Func<T, TValue> valueSelector, out TValue minValue)
            => enumerable.MinBy(valueSelector, out minValue, Comparer<TValue>.Default);

        public static T? MinByOrDefault<T, TValue>(this IEnumerable<T> enumerable, Func<T, TValue> valueSelector, out TValue? minValue, IComparer<TValue> comparer)
        {
            if (enumerable.Any() == false)
            {
                minValue = default;
                return default;
            }
            return enumerable.MinBy(valueSelector, out minValue, comparer);
        }
        public static T? MinByOrDefault<T, TValue>(this IEnumerable<T> enumerable, Func<T, TValue> valueSelector, out TValue? minValue)
            => enumerable.MinByOrDefault(valueSelector, out minValue, Comparer<TValue>.Default);

        public static int IndexOf<T>(this IEnumerable<T> enumerable, T item, IComparer<T> comparer)
        {
            int index = 0;
            foreach (T _item in enumerable)
            {
                if (comparer.Compare(_item, item) == 0)
                {
                    return index;
                }
                index++;
            }
            return -1;
        }
        public static int IndexOf<T>(this IEnumerable<T> enumerable, T item)
            => enumerable.IndexOf(item, Comparer<T>.Default);
        #endregion Search
    }
}
