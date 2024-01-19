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
    #region ConditionalWhere
    /// <summary>Will apply the predicate filter if the condition is true.</summary>
    public static IQueryable<TEntity> ConditionalWhere<TEntity>(this IQueryable<TEntity> source,
        bool condition,
        Expression<Func<TEntity, bool>> predicate
    )
        => source.ConditionallyApply(
            condition,
            (IQueryable<TEntity> query) => query.Where(predicate)
        );

    /// <summary>Will apply the predicate filter if the argument is not null nor an empty collection.</summary>
    public static IQueryable<TEntity> ConditionalWhere<TEntity, TArgument>(this IQueryable<TEntity> source,
        TArgument? argument,
        Func<TArgument, Expression<Func<TEntity, bool>>> createPredicate
    )
        where TArgument : struct
        => source.ConditionallyApply(
            argument,
            (TArgument argument, IQueryable<TEntity> query) => query.Where(createPredicate(argument))
        );

    /// <summary>Will apply the predicate filter if the argument is not null nor an empty collection.</summary>
    public static IQueryable<TEntity> ConditionalWhere<TEntity, TArgument>(this IQueryable<TEntity> source,
         TArgument? argument,
         Func<TArgument, Expression<Func<TEntity, bool>>> createPredicate
    )
         where TArgument : class
        => source.ConditionallyApply(
            argument,
            (TArgument argument, IQueryable<TEntity> query) => query.Where(createPredicate(argument))
        );

    /// <summary>Will apply the predicate filter if the arguments enumerable is not null nor empty.</summary>
    public static IQueryable<TEntity> ConditionalWhere<TEntity, TArgument>(this IQueryable<TEntity> source,
         IEnumerable<TArgument>? arguments,
         Func<IEnumerable<TArgument>, Expression<Func<TEntity, bool>>> createPredicate
    )
        => source.ConditionallyApply(
            arguments,
            (IEnumerable<TArgument> arguments, IQueryable<TEntity> query) => query.Where(createPredicate(arguments))
        );
    #endregion

    #region WhereAny
    public static IQueryable<TEntity> WhereAny<TEntity, TArgument>(this IQueryable<TEntity> source,
        IEnumerable<TArgument> arguments,
        Func<TArgument, Expression<Func<TEntity, bool>>> createPredicate
    )
    {
        Expression<Func<TEntity, bool>> any = arguments.ToAnyLambda(createPredicate);
        IQueryable<TEntity> query = source.Where(any);
        return query;
    }

    /// <summary>Will apply the predicate filter if the arguments enumerable is not null nor empty.</summary>
    public static IQueryable<TEntity> ConditionalWhereAny<TEntity, TArgument>(this IQueryable<TEntity> source,
         IEnumerable<TArgument>? arguments,
         Func<TArgument, Expression<Func<TEntity, bool>>> createPredicate
    )
        => source.ConditionallyApply(
            arguments,
            (IEnumerable<TArgument> _arguments, IQueryable<TEntity> query) => query.WhereAny(_arguments, createPredicate)
        );
    #endregion

    #region WhereEvery
    public static IQueryable<TEntity> WhereEvery<TEntity, TArgument>(this IQueryable<TEntity> source,
        IEnumerable<TArgument> arguments,
        Func<TArgument, Expression<Func<TEntity, bool>>> createPredicate
    )
    {
        Expression<Func<TEntity, bool>> every = arguments.ToEveryLambda(createPredicate);
        IQueryable<TEntity> query = source.Where(every);
        return query;
    }

    /// <summary>Will apply the predicate filter if the arguments enumerable is not null nor empty.</summary>
    public static IQueryable<TEntity> ConditionalWhereEvery<TEntity, TArgument>(this IQueryable<TEntity> source,
         IEnumerable<TArgument>? arguments,
         Func<TArgument, Expression<Func<TEntity, bool>>> createPredicate
    )
        => source.ConditionallyApply(
            arguments,
            (IEnumerable<TArgument> _arguments, IQueryable<TEntity> query) => query.WhereEvery(_arguments, createPredicate)
        );
    #endregion

    public static IQueryable<TEntity> WhereAnyPhrase<TEntity>(this IQueryable<TEntity> source,
        string text,
        Func<string, Expression<Func<TEntity, bool>>> predicateBuilder,
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
        IQueryable<TEntity> query = source.WhereAny(phrases, predicateBuilder);
        return query;
    }

    public static IQueryable<TEntity> WhereEveryPhrase<TEntity>(this IQueryable<TEntity> source,
        string? text,
        Func<string, Expression<Func<TEntity, bool>>> predicateBuilder,
        char wordSeparator = ' ',
        char phraseSeparator = '"',
        StringSplitOptions splitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
    )
    {
        if (string.IsNullOrEmpty(text))
            return source;
        IEnumerable<string> phrases = text.SplitToPhraseEnumerable(wordSeparator, phraseSeparator, splitOptions);
        IQueryable<TEntity> query = source.WhereEvery(phrases, predicateBuilder);
        return query;
    }

    #region SetOrder
    public static IOrderedQueryable<TEntity> SetOrder<TEntity>(this IQueryable<TEntity> query,
        OrderDirection direction,
        Expression<Func<TEntity, object?>> property,
        params Expression<Func<TEntity, object?>>[] additionalProperties
    )
    {
        Func<IQueryable<TEntity>, Expression<Func<TEntity, object?>>, IOrderedQueryable<TEntity>> orderBy;
        Func<IOrderedQueryable<TEntity>, Expression<Func<TEntity, object?>>, IOrderedQueryable<TEntity>> thenOrderBy;
        switch (direction)
        {
            case OrderDirection.Ascending:
                orderBy = Queryable.OrderBy<TEntity, object?>;
                thenOrderBy = Queryable.ThenBy<TEntity, object?>;
                break;
            case OrderDirection.Descending:
                orderBy = Queryable.OrderByDescending<TEntity, object?>;
                thenOrderBy = Queryable.ThenByDescending<TEntity, object?>;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(direction));
        }

        IOrderedQueryable<TEntity> orderedQuery = orderBy(query, property);
        foreach (Expression<Func<TEntity, object?>> _property in additionalProperties)
        {
            orderedQuery = thenOrderBy(orderedQuery, _property);
        }

        return orderedQuery;
    }

    public static IOrderedQueryable<TEntity> SetOrder<TEntity>(this IQueryable<TEntity> query,
        OrderDirection direction,
        PropertySelection<TEntity> properties
    )
        => query.SetOrder(direction, properties.Property, properties.AdditionalProperties);
    #endregion

    #region ToImmutableArrayAsync
    public static Task<ImmutableArray<T>> ToImmutableArrayAsync<T>(this IQueryable<T> query,
        CancellationToken cancellationToken = default
    )
        => query.ToImmutableArrayAsync(ImmutableArray.CreateBuilder<T>(), cancellationToken);

    public static Task<ImmutableArray<T>> ToImmutableArrayAsync<T>(this IQueryable<T> query,
        int initialCapacity,
        CancellationToken cancellationToken = default
    )
        => query.ToImmutableArrayAsync(ImmutableArray.CreateBuilder<T>(initialCapacity), cancellationToken);

    private static async Task<ImmutableArray<T>> ToImmutableArrayAsync<T>(this IQueryable<T> query, ImmutableArray<T>.Builder builder, CancellationToken cancellationToken = default)
    {
        ConfiguredCancelableAsyncEnumerable<T> asyncEnumerable = query.AsAsyncEnumerable().WithCancellation(cancellationToken);
        await foreach (T item in asyncEnumerable)
            builder.Add(item);
        ImmutableArray<T> immutableArray = builder.ToImmutable();
        return immutableArray;
    }
    #endregion

    #region ToImmutableDictionaryAsync
    public static Task<ImmutableDictionary<TKey, TEntity>> ToImmutableDictionaryAsync<TEntity, TKey>(this IQueryable<TEntity> query,
        Func<TEntity, TKey> keySelector,
        IEqualityComparer<TKey>? keyComparer = null,
        IEqualityComparer<TEntity>? entityComparer = null,
        CancellationToken cancellationToken = default
    )
        where TKey : notnull
        => query.ToImmutableDictionaryAsync(keySelector, (entity) => entity, keyComparer, entityComparer, cancellationToken);

    public static async Task<ImmutableDictionary<TKey, TValue>> ToImmutableDictionaryAsync<TEntity, TKey, TValue>(this IQueryable<TEntity> query,
        Func<TEntity, TKey> keySelector,
        Func<TEntity, TValue> valueSelector,
        IEqualityComparer<TKey>? keyComparer = null,
        IEqualityComparer<TValue>? valueComparer = null,
        CancellationToken cancellationToken = default
    )
        where TKey : notnull
    {
        ImmutableDictionary<TKey, TValue>.Builder builder = ImmutableDictionary.CreateBuilder(keyComparer, valueComparer);
        ConfiguredCancelableAsyncEnumerable<TEntity> asyncEnumerable = query.AsAsyncEnumerable().WithCancellation(cancellationToken);
        await foreach (TEntity entity in asyncEnumerable)
            builder.Add(keySelector(entity), valueSelector(entity));
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
    public static async Task<Page<TEntity>> ToPageAsync<TEntity>(this IQueryable<TEntity> query, uint pageSize, uint pageNumber, CancellationToken cancellationToken = default)
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
        ulong totalCount = (ulong)await query.LongCountAsync(cancellationToken);
        int _pageSize = (int)pageSize;
        ImmutableArray<TEntity> entities = await query
            .Skip((int)skipLong)
            .Take(_pageSize)
            .ToImmutableArrayAsync(_pageSize, cancellationToken);
        Page<TEntity> page = new(entities, totalCount, pageSize);
        return page;
    }

    #region WhereAnyAsync
    public static async Task<ImmutableArray<TEntity>> WhereAnyAsync<TEntity, TData>(this IQueryable<TEntity> query,
        IEnumerable<TData> dataSource,
        int chunkSize,
        Func<TData, Expression<Func<TEntity, bool>>> predicateBuilder,
        CancellationToken cancellationToken = default
    )
    {
        List<TEntity> entitiesBuffer = new(dataSource.Count());
        foreach (TData[] dataChunk in dataSource.Chunk(chunkSize))
        {
            List<TEntity> entityChunk = await query
                .WhereAny(dataChunk, predicateBuilder)
                .ToListAsync(cancellationToken);
            entitiesBuffer.AddRange(entityChunk);
        }
        ImmutableArray<TEntity> entities = entitiesBuffer.Distinct().ToImmutableArray();
        return entities;
    }

    public static async Task<ImmutableArray<TResult>> WhereAnyAsync<TEntity, TData, TResult>(this IQueryable<TEntity> query,
        IEnumerable<TData> dataSource,
        int chunkSize,
        Func<TData, Expression<Func<TEntity, bool>>> predicateBuilder,
        Func<IQueryable<TEntity>, IQueryable<TResult>> additionalOperations,
        CancellationToken cancellationToken = default
    )
    {
        List<TResult> resultBuffer = new(dataSource.Count());
        foreach (TData[] dataChunk in dataSource.Chunk(chunkSize))
        {
            IQueryable<TResult> resultQuery = additionalOperations(query.WhereAny(dataChunk, predicateBuilder));
            List<TResult> resultChunk = await resultQuery.ToListAsync(cancellationToken);
            resultBuffer.AddRange(resultChunk);
        }
        ImmutableArray<TResult> results = resultBuffer.Distinct().ToImmutableArray();
        return results;
    }
    #endregion
}
