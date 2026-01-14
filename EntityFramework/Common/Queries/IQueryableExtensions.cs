using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

using AndrejKrizan.DotNet.Collections;
using AndrejKrizan.DotNet.Functional;
using AndrejKrizan.DotNet.Lambdas;
using AndrejKrizan.DotNet.Ordering;
using AndrejKrizan.DotNet.Pagination;
using AndrejKrizan.DotNet.Strings;

using Microsoft.EntityFrameworkCore;

namespace AndrejKrizan.EntityFramework.Common.Queries;

public static class IQueryableExtensions
{
    extension<T>(IQueryable<T> source)
    {
        #region ConditionalWhere
        /// <summary>Will apply the predicate filter if the condition is true.</summary>
        public IQueryable<T> ConditionalWhere(bool condition, Expression<Func<T, bool>> predicate)
            => source.ConditionallyApply(condition, (IQueryable<T> query) 
                => query.Where(predicate)
            );

        public IQueryable<T> ConditionalWhere(string? argument, Func<string, Expression<Func<T, bool>>> createPredicate)
            => source.ConditionallyApply(argument, (string argument, IQueryable<T> query) 
                => query.Where(createPredicate(argument))
            );

        /// <summary>Will apply the predicate filter if the argument is not null nor an empty collection.</summary>
        public IQueryable<T> ConditionalWhere<TArgument>(TArgument? argument, Func<TArgument, Expression<Func<T, bool>>> createPredicate)
            where TArgument : struct
            => source.ConditionallyApply(argument, (TArgument argument, IQueryable<T> query) 
                => query.Where(createPredicate(argument))
            );

        /// <summary>Will apply the predicate filter if the argument is not null nor an empty collection.</summary>
        public IQueryable<T> ConditionalWhere<TArgument>(TArgument? argument, Func<TArgument, Expression<Func<T, bool>>> createPredicate)
             where TArgument : class
            => source.ConditionallyApply(argument, (TArgument argument, IQueryable<T> query) 
                => query.Where(createPredicate(argument))
            );

        /// <summary>Will apply the predicate filter if the arguments enumerable is not null nor empty.</summary>
        public IQueryable<T> ConditionalWhere<TArgument>(IEnumerable<TArgument>? arguments, Func<IEnumerable<TArgument>, Expression<Func<T, bool>>> createPredicate)
            => source.ConditionallyApply(arguments, (IEnumerable<TArgument> arguments, IQueryable<T> query) 
                => query.Where(createPredicate(arguments))
            );
        #endregion

        #region WhereAny
        public IQueryable<T> WhereAny<TArgument>(IEnumerable<TArgument> arguments, Func<TArgument, Expression<Func<T, bool>>> createPredicate)
        {
            Expression<Func<T, bool>> any = arguments.ToAnyLambda(createPredicate);
            IQueryable<T> query = source.Where(any);
            return query;
        }

        /// <summary>Will apply the predicate filter if the arguments enumerable is not null nor empty.</summary>
        public IQueryable<T> ConditionalWhereAny<TArgument>(IEnumerable<TArgument>? arguments, Func<TArgument, Expression<Func<T, bool>>> createPredicate)
            => source.ConditionallyApply(arguments, (IEnumerable<TArgument> arguments, IQueryable<T> query)
                => query.WhereAny(arguments, createPredicate)
            );
        #endregion

        #region WhereEvery
        public IQueryable<T> WhereEvery<TArgument>(IEnumerable<TArgument> arguments, Func<TArgument, Expression<Func<T, bool>>> createPredicate)
        {
            Expression<Func<T, bool>> every = arguments.ToEveryLambda(createPredicate);
            IQueryable<T> query = source.Where(every);
            return query;
        }

        /// <summary>Will apply the predicate filter if the arguments enumerable is not null nor empty.</summary>
        public IQueryable<T> ConditionalWhereEvery<TArgument>(IEnumerable<TArgument>? arguments, Func<TArgument, Expression<Func<T, bool>>> createPredicate)
            => source.ConditionallyApply(arguments, (IEnumerable<TArgument> arguments, IQueryable<T> query)
                => query.WhereEvery(arguments, createPredicate)
            );
        #endregion

        public IQueryable<T> WhereAnyPhrase(string? text, Func<string, Expression<Func<T, bool>>> predicateBuilder,
            char wordSeparator = ' ',
            char phraseSeparator = '"',
            StringSplitOptions splitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
        )
        {
            if (string.IsNullOrEmpty(text))
            {
                return source;
            }
            IEnumerable<string> phrases = text.SplitToPhraseEnumerable(wordSeparator, phraseSeparator, splitOptions);
            IQueryable<T> query = source.ConditionalWhereAny(phrases, predicateBuilder);
            return query;
        }

        public IQueryable<T> WhereEveryPhrase(string? text, Func<string, Expression<Func<T, bool>>> predicateBuilder,
            char wordSeparator = ' ',
            char phraseSeparator = '"',
            StringSplitOptions splitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
        )
        {
            if (string.IsNullOrEmpty(text))
            {
                return source;
            }
            IEnumerable<string> phrases = text.SplitToPhraseEnumerable(wordSeparator, phraseSeparator, splitOptions);
            IQueryable<T> query = source.ConditionalWhereEvery(phrases, predicateBuilder);
            return query;
        }

        #region SetOrder
        public IOrderedQueryable<T> SetOrder(OrderDirection direction, Expression<Func<T, object?>> property,
            params Expression<Func<T, object?>>[] additionalProperties
        )
        {
            Func<IQueryable<T>, Expression<Func<T, object?>>, IOrderedQueryable<T>> orderBy;
            Func<IOrderedQueryable<T>, Expression<Func<T, object?>>, IOrderedQueryable<T>> thenOrderBy;
            switch (direction)
            {
                case OrderDirection.Ascending:
                    orderBy = Queryable.OrderBy<T, object?>;
                    thenOrderBy = Queryable.ThenBy<T, object?>;
                    break;
                case OrderDirection.Descending:
                    orderBy = Queryable.OrderByDescending<T, object?>;
                    thenOrderBy = Queryable.ThenByDescending<T, object?>;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction));
            }

            IOrderedQueryable<T> orderedQuery = orderBy(source, property);
            foreach (Expression<Func<T, object?>> _property in additionalProperties)
            {
                orderedQuery = thenOrderBy(orderedQuery, _property);
            }

            return orderedQuery;
        }

        public IOrderedQueryable<T> SetOrder(OrderDirection direction, PropertySelection<T> properties)
            => source.SetOrder(direction, properties.Property, properties.AdditionalProperties);
        #endregion

        #region ToImmutableArrayAsync
        public Task<ImmutableArray<T>> ToImmutableArrayAsync(CancellationToken cancellationToken = default)
            => source.ToImmutableArrayAsync(ImmutableArray.CreateBuilder<T>(), cancellationToken);

        public Task<ImmutableArray<T>> ToImmutableArrayAsync(int initialCapacity, CancellationToken cancellationToken = default)
            => source.ToImmutableArrayAsync(ImmutableArray.CreateBuilder<T>(initialCapacity), cancellationToken);

        private async Task<ImmutableArray<T>> ToImmutableArrayAsync(ImmutableArray<T>.Builder builder, CancellationToken cancellationToken = default)
        {
            ConfiguredCancelableAsyncEnumerable<T> asyncEnumerable = source.AsAsyncEnumerable().WithCancellation(cancellationToken);
            await foreach (T item in asyncEnumerable)
            {
                builder.Add(item);
            }
            ImmutableArray<T> immutableArray = builder.ToImmutable();
            return immutableArray;
        }
        #endregion

        #region ToImmutableDictionaryAsync
        public Task<ImmutableDictionary<TKey, T>> ToImmutableDictionaryAsync<TKey>(Func<T, TKey> keySelector,
            IEqualityComparer<TKey>? keyComparer = null, IEqualityComparer<T>? entityComparer = null,
            CancellationToken cancellationToken = default
        )
            where TKey : notnull
            => source.ToImmutableDictionaryAsync(
                keySelector, (T item) => item, 
                keyComparer, entityComparer, 
                cancellationToken
            );

        public async Task<ImmutableDictionary<TKey, TValue>> ToImmutableDictionaryAsync<TKey, TValue>(Func<T, TKey> keySelector, Func<T, TValue> valueSelector,
            IEqualityComparer<TKey>? keyComparer = null, IEqualityComparer<TValue>? valueComparer = null,
            CancellationToken cancellationToken = default
        )
            where TKey : notnull
        {
            ImmutableDictionary<TKey, TValue>.Builder builder = ImmutableDictionary.CreateBuilder(keyComparer, valueComparer);
            ConfiguredCancelableAsyncEnumerable<T> asyncEnumerable = source.AsAsyncEnumerable().WithCancellation(cancellationToken);
            await foreach (T item in asyncEnumerable)
            {
                TKey key = keySelector(item);
                TValue value = valueSelector(item);
                builder.Add(key, value);
            }
            ImmutableDictionary<TKey, TValue> immutableDictionary = builder.ToImmutable();
            return immutableDictionary;
        }
        #endregion

        /// <param name="pageSize">Positive integer not larger than <see cref="int.MaxValue"/></param>
        /// <param name="pageNumber">Positive integer not larger than <see cref="int.MaxValue"/></param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Page size must be a positive integer not larger than int.MaxValue.
        ///     Page number must be a positive integer not larger than int.MaxValue.
        ///     (pageNumber - 1) * pageSize cannot be larger than int.MaxValue due to Entity framework core restriction.
        /// </exception>
        public async Task<Page<T>> ToPageAsync(uint pageSize, uint pageNumber, CancellationToken cancellationToken = default)
        {
            if (pageSize is < 1 or > int.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(pageSize), $"Page size must be a positive integer not larger than int.MaxValue ({int.MaxValue}).");
            if (pageNumber is < 1 or > int.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(pageNumber), $"Page number must be a positive integer not larger than int.MaxValue ({int.MaxValue}).");
            ulong skipLong = (ulong)pageSize * (pageNumber - 1);
            if (skipLong > int.MaxValue)
                throw new ArgumentOutOfRangeException(
                    paramName: null,
                    message: $"({nameof(pageNumber)} - 1) * {nameof(pageSize)} cannot be larger than int.MaxValue ({int.MaxValue}) due to Entity framework core restriction."
                );
            ulong totalCount = (ulong)await source.LongCountAsync(cancellationToken);
            int _pageSize = (int)pageSize;
            ImmutableArray<T> results = await source
                .Skip((int)skipLong)
                .Take(_pageSize)
                .ToImmutableArrayAsync(_pageSize, cancellationToken);
            Page<T> page = new(results, totalCount, pageSize);
            return page;
        }

        #region MinOrDefaultAsync

        /// <returns>A task whose result contains the minimum value in the sequence or the default value of <typeparamref name="TValue"/> if the sequence is empty.</returns>
        /// <exception cref="OperationCanceledException">If the <see cref="CancellationToken" /> is canceled.</exception>
        public async Task<TValue> MinOrDefaultAsync<TValue>(Expression<Func<T, TValue>> selector, CancellationToken cancellationToken = default)
            where TValue : struct
            => await source.MinOrDefaultAsync(selector, defaultValue: default, cancellationToken);

        /// <returns>A task whose result contains the minimum value in the sequence or the <paramref name="defaultValue"/> if the sequence is empty.</returns>
        /// <exception cref="OperationCanceledException">If the <see cref="CancellationToken" /> is canceled.</exception>
        public async Task<TValue> MinOrDefaultAsync<TValue>(Expression<Func<T, TValue>> selector, TValue defaultValue, CancellationToken cancellationToken = default)
            where TValue : struct
            => await source.Select(selector).MinOrDefaultAsync(defaultValue, cancellationToken);

        #endregion


        #region MaxOrDefaultAsync

        /// <returns>A task whose result contains the maximum value in the sequence or the default value of <typeparamref name="TValue"/> if the sequence is empty.</returns>
        /// <exception cref="OperationCanceledException">If the <see cref="CancellationToken" /> is canceled.</exception>
        public async Task<TValue> MaxOrDefaultAsync<TValue>(Expression<Func<T, TValue>> selector, CancellationToken cancellationToken = default)
            where TValue : struct
            => await source.MaxOrDefaultAsync(selector, defaultValue: default, cancellationToken);

        /// <returns>A task whose result contains the maximum value in the sequence or the <paramref name="defaultValue"/> if the sequence is empty.</returns>
        /// <exception cref="OperationCanceledException">If the <see cref="CancellationToken" /> is canceled.</exception>
        public async Task<TValue> MaxOrDefaultAsync<TValue>(Expression<Func<T, TValue>> selector, TValue defaultValue, CancellationToken cancellationToken = default)
            where TValue : struct
            => await source.Select(selector).MaxOrDefaultAsync(defaultValue, cancellationToken);

        #endregion


        #region WhereAnyAsync
        public async Task<ImmutableArray<T>> WhereAnyAsync<TData>(
            IEnumerable<TData> dataSource, int chunkSize, Func<TData, Expression<Func<T, bool>>> predicateBuilder, 
            CancellationToken cancellationToken = default
        )
        {
            List<T> resultBuffer = new(dataSource.Count());
            foreach (TData[] dataChunk in dataSource.Chunk(chunkSize))
            {
                List<T> resultChunk = await source
                    .WhereAny(dataChunk, predicateBuilder)
                    .ToListAsync(cancellationToken);
                resultBuffer.AddRange(resultChunk);
            }
            ImmutableArray<T> results = resultBuffer.Distinct().ToImmutableArray();
            return results;
        }

        public async Task<ImmutableArray<TResult>> WhereAnyAsync<TData, TResult>(
            IEnumerable<TData> dataSource, int chunkSize, Func<TData, Expression<Func<T, bool>>> predicateBuilder,
            Func<IQueryable<T>, IQueryable<TResult>> additionalOperations,
            CancellationToken cancellationToken = default
        )
        {
            List<TResult> resultBuffer = new(dataSource.Count());
            foreach (TData[] dataChunk in dataSource.Chunk(chunkSize))
            {
                IQueryable<TResult> resultQuery = additionalOperations(source.WhereAny(dataChunk, predicateBuilder));
                List<TResult> resultChunk = await resultQuery.ToListAsync(cancellationToken);
                resultBuffer.AddRange(resultChunk);
            }
            ImmutableArray<TResult> results = resultBuffer.Distinct().ToImmutableArray();
            return results;
        }
        #endregion
    }

    extension<TValue>(IQueryable<TValue> source)
        where TValue: struct
    {
        #region MinOrDefaultAsync

        /// <returns>A task whose result contains the minimum value in the sequence or the default value of <typeparamref name="TValue"/> if the sequence is empty.</returns>
        /// <exception cref="OperationCanceledException">If the <see cref="CancellationToken" /> is canceled.</exception>
        public async Task<TValue> MinOrDefaultAsync(CancellationToken cancellationToken = default)
            => await source.MinOrDefaultAsync(defaultValue: default, cancellationToken);

        /// <returns>A task whose result contains the minimum value in the sequence or the <paramref name="defaultValue"/> if the sequence is empty.</returns>
        /// <exception cref="OperationCanceledException">If the <see cref="CancellationToken" /> is canceled.</exception>
        public async Task<TValue> MinOrDefaultAsync(TValue defaultValue = default, CancellationToken cancellationToken = default)
            => await source.MinAsync((TValue value) => (TValue?)value, cancellationToken) ?? defaultValue;

        #endregion

        #region MaxOrDefaultAsync

        /// <returns>A task whose result contains the maximum value in the sequence or the default value of <typeparamref name="TValue"/> if the sequence is empty.</returns>
        /// <exception cref="OperationCanceledException">If the <see cref="CancellationToken" /> is canceled.</exception>
        public async Task<TValue> MaxOrDefaultAsync(CancellationToken cancellationToken = default)
            => await source.MaxOrDefaultAsync(defaultValue: default, cancellationToken);

        /// <returns>A task whose result contains the maximum value in the sequence or the <paramref name="defaultValue"/> if the sequence is empty.</returns>
        /// <exception cref="OperationCanceledException">If the <see cref="CancellationToken" /> is canceled.</exception>
        public async Task<TValue> MaxOrDefaultAsync(TValue defaultValue, CancellationToken cancellationToken = default)
            => await source.MaxAsync((TValue value) => (TValue?)value, cancellationToken) ?? defaultValue;
            
        #endregion
    }
}
