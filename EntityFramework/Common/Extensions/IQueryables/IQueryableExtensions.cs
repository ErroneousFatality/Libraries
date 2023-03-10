using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

using AndrejKrizan.DotNet.Extensions;
using AndrejKrizan.DotNet.ValueObjects;
using AndrejKrizan.EntityFramework.Common.Extensions.Lambda;

using Microsoft.EntityFrameworkCore;

namespace AndrejKrizan.EntityFramework.Common.Extensions.IQueryables
{
    public static class IQueryableExtensions
    {
        // Static properties
        private static readonly MethodInfo StringStartsWithMethodInfo = typeof(string).GetMethod(nameof(string.StartsWith), new Type[] { typeof(string) })!;
        private static readonly MethodInfo StringContainsMethodInfo = typeof(string).GetMethod(nameof(string.Contains), new Type[] { typeof(string) })!;
        private static readonly MethodInfo StringEndsWithMethodInfo = typeof(string).GetMethod(nameof(string.EndsWith), new Type[] { typeof(string) })!;
        private static readonly MethodInfo[] SupportedStringMethodInfos = new MethodInfo[] { StringStartsWithMethodInfo, StringContainsMethodInfo, StringEndsWithMethodInfo };

        // Methods

        #region ConditionalWhere
        public static IQueryable<TEntity> ConditionalWhere<TEntity>(this IQueryable<TEntity> query,
            bool condition,
            Expression<Func<TEntity, bool>> predicate
        )
        {
            if (!condition)
            {
                return query;
            }
            return query.Where(predicate);
        }

        public static IQueryable<TEntity> ConditionalWhere<TEntity, T>(this IQueryable<TEntity> query,
            T? nullable,
            Func<T, Expression<Func<TEntity, bool>>> predicateBuilder
        )
            where T : struct
        {
            if (nullable is null)
            {
                return query;
            }
            Expression<Func<TEntity, bool>> predicate = predicateBuilder(nullable.Value);
            return query.Where(predicate);
        }

        public static IQueryable<TEntity> ConditionalWhere<TEntity, T>(this IQueryable<TEntity> query,
             T? nullable,
             Func<T, Expression<Func<TEntity, bool>>> predicateBuilder
        )
             where T : class
        {
            if (nullable is null)
            {
                return query;
            }
            Expression<Func<TEntity, bool>> predicate = predicateBuilder(nullable);
            return query.Where(predicate);
        }

        public static IQueryable<TEntity> ConditionalWhere<TEntity, T>(this IQueryable<TEntity> query,
             IEnumerable<T> enumerable,
             Func<IEnumerable<T>, Expression<Func<TEntity, bool>>> predicateBuilder
        )
        {
            if (!enumerable.Any())
            {
                return query;
            }
            Expression<Func<TEntity, bool>> predicate = predicateBuilder(enumerable);
            return query.Where(predicate);
        }
        #endregion

        public static IQueryable<TEntity> WhereAny<TEntity, TData>(this IQueryable<TEntity> query,
            IEnumerable<TData> dataSource,
            Func<TData, Expression<Func<TEntity, bool>>> predicateBuilder
        )
        {
            if (dataSource.Any())
            {
                Expression<Func<TEntity, bool>> predicate = dataSource.ToPredicateLambda(predicateBuilder);
                query = query.Where(predicate);
            }
            return query;
        }

        #region String filtering
        /// <summary>
        ///     Filters the query by whether every word in the filter is start of any of the entity's selected properties.
        /// </summary>
        public static IQueryable<TEntity> WhereStartsWith<TEntity>(
            this IQueryable<TEntity> query,
            string? filter,
            Expression<Func<TEntity, string?>> stringPropertyLambda,
            params Expression<Func<TEntity, string?>>[] additionalStringPropertyLambdas
        )
            where TEntity : class
            => query.WhereFilterUsingMethod(filter, StringStartsWithMethodInfo, stringPropertyLambda, additionalStringPropertyLambdas);

        /// <summary>
        ///     Filters the query by whether every word in the filter is contained in any of the entity's selected properties.
        /// </summary>
        public static IQueryable<TEntity> WhereContains<TEntity>(
            this IQueryable<TEntity> query,
            string? filter,
            Expression<Func<TEntity, string?>> stringPropertyLambda,
            params Expression<Func<TEntity, string?>>[] additionalStringPropertyLambdas
        )
            where TEntity : class
            => query.WhereFilterUsingMethod(filter, StringContainsMethodInfo, stringPropertyLambda, additionalStringPropertyLambdas);

        /// <summary>
        ///     Filters the query by whether every word in the filter is end of any of the entity's selected properties.
        /// </summary>
        public static IQueryable<TEntity> WhereEndsWith<TEntity>(
            this IQueryable<TEntity> query,
            string? filter,
            Expression<Func<TEntity, string?>> stringPropertyLambda,
            params Expression<Func<TEntity, string?>>[] additionalStringPropertyLambdas
        )
            where TEntity : class
            => query.WhereFilterUsingMethod(filter, StringEndsWithMethodInfo, stringPropertyLambda, additionalStringPropertyLambdas);


        /// <summary>
        ///     Example of a string property method navigation expression: (TEntity entity) => entity.Property1...PropertyN.StringProperty.BoolMethod.
        ///     Supported string bool methods: StartsWith, Contains, EndsWith.
        /// </summary>
        /// <exception cref="ArgumentException">
        ///     Lambda must point to a method belonging to a property.
        ///     Expression does not represent a property navigation expression.
        ///     String property method navigation expression must point to one of these supported methods: StartsWith, Cointains, EndsWith.
        /// </exception>
        public static IQueryable<TEntity> WhereFilter<TEntity>(
            this IQueryable<TEntity> query,
            string? filter,
            Expression<Func<TEntity, Func<string, bool>>> stringPropertyMethodNavigationExpression,
            params Expression<Func<TEntity, Func<string, bool>>>[] additionalStringPropertyMethodNavigationExpression
        )
            where TEntity : class
        {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(TEntity), "entity");
            IEnumerable<PropertyNavigationExpressionAndMethodInfo<TEntity, string>> stringPropertyNavigationExpressionAndMethodInfoEnumerable = additionalStringPropertyMethodNavigationExpression
                .Prepend(stringPropertyMethodNavigationExpression)
                .Select(_stringPropertyMethodNavigationExpression =>
                {
                    PropertyNavigationExpressionAndMethodInfo<TEntity, string> propertyNavigationExpressionAndMethodInfo = new(_stringPropertyMethodNavigationExpression, parameterExpression);
                    if (!SupportedStringMethodInfos.Contains(propertyNavigationExpressionAndMethodInfo.MethodInfo))
                    {
                        string supportedStringMethodNamesString = string.Join(", ", SupportedStringMethodInfos.Select((methodInfo) => methodInfo.Name));
                        throw new ArgumentException($"String property method navigation expression must point to one of these supported methods: {supportedStringMethodNamesString}.");
                    }
                    return propertyNavigationExpressionAndMethodInfo;
                }
            );
            return query.WhereFilterUsingMultipleMethods(filter, parameterExpression, stringPropertyNavigationExpressionAndMethodInfoEnumerable);
        }

        private static IQueryable<TEntity> WhereFilterUsingMethod<TEntity>(
            this IQueryable<TEntity> query,
            string? filter,
            MethodInfo methodInfo,
            Expression<Func<TEntity, string?>> stringPropertyLambda,
            params Expression<Func<TEntity, string?>>[] additionalStringPropertyLambdas
        )
            where TEntity : class
        {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(TEntity), "entity");
            IEnumerable<PropertyNavigationExpressionAndMethodInfo<TEntity, string>> stringPropertyNavigationExpressionAndMethodInfoCollection = additionalStringPropertyLambdas
                .Prepend(stringPropertyLambda)
                .Select(_stringPropertyLambda =>
                    new PropertyNavigationExpressionAndMethodInfo<TEntity, string>(
                        new PropertyNavigationExpression<TEntity, string?>(_stringPropertyLambda, parameterExpression),
                        methodInfo
                    )
                );
            return query.WhereFilterUsingMultipleMethods(filter, parameterExpression, stringPropertyNavigationExpressionAndMethodInfoCollection);
        }

        /// <exception cref="ArgumentException">
        ///     Lambda expression does not represent a property navigation expression.
        /// </exception>
        private static IQueryable<TEntity> WhereFilterUsingMultipleMethods<TEntity>(
            this IQueryable<TEntity> query,
            string? filter,
            ParameterExpression parameterExpression,
            IEnumerable<PropertyNavigationExpressionAndMethodInfo<TEntity, string>> stringPropertyNavigationExpressionAndMethodInfoEnumerable
        )
            where TEntity : class
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                return query;
            }
            string[] words = filter.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            List<string> phrases = new(words.Length);
            int? phraseStartIndex = null;
            int phraseWordCount = 0;
            for (int i = 0; i < words.Length; i++)
            {
                string word = words[i];
                if (phraseStartIndex == null)
                {
                    if (word.StartsWith('"'))
                    {
                        if (word.EndsWith('"'))
                        {
                            string phrase = word[1..^1];
                            phrases.Add(phrase);
                        }
                        else
                        {
                            words[i] = word[1..];
                            phraseStartIndex = i;
                            phraseWordCount = 1;
                        }

                    }
                    else
                    {
                        phrases.Add(word);
                    }
                }
                else
                {
                    phraseWordCount++;
                    if (word.EndsWith('"'))
                    {
                        words[i] = word[0..^1];
                        string phrase = words.Skip(phraseStartIndex.Value).Take(phraseWordCount).StringJoin(separator: ' ');
                        phraseStartIndex = null;
                        phraseWordCount = 0;
                        phrases.Add(phrase);
                    }
                }
            }

            Expression everyWordIsMatchedInAnyStringProperty = phrases.Select((string word) =>
            {
                ConstantExpression wordExpression = Expression.Constant(word);
                Expression anyStringMemberMatchesFilter = stringPropertyNavigationExpressionAndMethodInfoEnumerable
                    .Select(propertyNavigationExpressionAndMethodInfo =>
                        Expression.Call(
                            propertyNavigationExpressionAndMethodInfo.PropertyNavigationExpression.Expression,
                            propertyNavigationExpressionAndMethodInfo.MethodInfo,
                            wordExpression
                        )
                    )
                    .Aggregate<Expression>(Expression.OrElse);
                return anyStringMemberMatchesFilter;
            }).Aggregate(Expression.AndAlso);
            Expression<Func<TEntity, bool>> stringFilterLambda = Expression.Lambda<Func<TEntity, bool>>(
                everyWordIsMatchedInAnyStringProperty,
                parameterExpression
            );
            return query.Where(stringFilterLambda);
        }
        #endregion String filtering

        #region Ordering
        /// <exception cref="ArgumentOutOfRangeException">orderDirection</exception>
        public static IOrderedQueryable<TEntity> SetOrder<TEntity>(
            this IQueryable<TEntity> query,
            OrderDirection orderDirection,
            Expression<Func<TEntity, object>> propertySelector,
            params Expression<Func<TEntity, object>>[] additionalPropertySelectors
        )
            where TEntity : class
        {
            Func<IQueryable<TEntity>, Expression<Func<TEntity, object>>, IOrderedQueryable<TEntity>> orderBy;
            Func<IOrderedQueryable<TEntity>, Expression<Func<TEntity, object>>, IOrderedQueryable<TEntity>> thenOrderBy;
            switch (orderDirection)
            {
                case OrderDirection.Ascending:
                    orderBy = Queryable.OrderBy<TEntity, object>;
                    thenOrderBy = Queryable.ThenBy<TEntity, object>;
                    break;
                case OrderDirection.Descending:
                    orderBy = Queryable.OrderByDescending<TEntity, object>;
                    thenOrderBy = Queryable.ThenByDescending<TEntity, object>;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(orderDirection));
            }
            IOrderedQueryable<TEntity> orderedQuery = orderBy(query, propertySelector);
            foreach (Expression<Func<TEntity, object>> _propertySelector in additionalPropertySelectors)
            {
                orderedQuery = thenOrderBy(orderedQuery, _propertySelector);
            }
            return orderedQuery;
        }

        public static IOrderedQueryable<TEntity> SetOrder<TEntity>(
            this IQueryable<TEntity> query,
            OrderDirection orderDirection,
            PropertySelection<TEntity> propertySelectors
        )
            where TEntity : class
            => query.SetOrder(orderDirection, propertySelectors.PropertySelector, propertySelectors.AdditionalPropertySelectors);
        #endregion Ordering

        #region ToImmutableArrayAsync
        public static async Task<ImmutableArray<T>> ToImmutableArrayAsync<T>(this IQueryable<T> query,
            CancellationToken cancellationToken = default
        )
        {
            ImmutableArray<T>.Builder builder = ImmutableArray.CreateBuilder<T>();
            ConfiguredCancelableAsyncEnumerable<T> asyncEnumerable = query.AsAsyncEnumerable().WithCancellation(cancellationToken);
            await foreach (T item in asyncEnumerable)
            {
                builder.Add(item);
            }
            ImmutableArray<T> immutableArray = builder.ToImmutable();
            return immutableArray;
        }

        public static async Task<ImmutableArray<T>> ToImmutableArrayAsync<T>(this IQueryable<T> query,
            int initialCapacity,
            CancellationToken cancellationToken = default
        )
        {
            ImmutableArray<T>.Builder builder = ImmutableArray.CreateBuilder<T>(initialCapacity);
            ConfiguredCancelableAsyncEnumerable<T> asyncEnumerable = query.AsAsyncEnumerable().WithCancellation(cancellationToken);
            await foreach (T item in asyncEnumerable)
            {
                builder.Add(item);
            }
            ImmutableArray<T> immutableArray = builder.ToImmutable();
            return immutableArray;
        }
        #endregion

        /// <param name="pageSize">Positive integer not larger than <see cref="int.MaxValue."/></param>
        /// <param name="pageNumber">Positive integer not larger than <see cref="int.MaxValue."/></param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Page size must be a positive integer not larger than int.MaxValue.
        ///     Page number must be a positive integer not larger than int.MaxValue.
        ///     (pageNumber - 1) * pageSize cannot be larger than int.MaxValue due to Entity framework core restriction.
        /// </exception>
        public static async Task<Page<T>> ToPageAsync<T>(this IQueryable<T> query, uint pageSize, uint pageNumber, CancellationToken cancellationToken = default)
        {
            if (pageSize < 1 || pageSize > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize), $"Page size must be a positive integer not larger than int.MaxValue ({int.MaxValue}).");
            }
            if (pageNumber < 1 || pageNumber > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(pageNumber), $"Page number must be a positive integer not larger than int.MaxValue ({int.MaxValue}).");
            }
            ulong skipLong = (ulong)pageSize * (pageNumber - 1);
            if (skipLong > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(
                    paramName: null,
                    message: $"({nameof(pageNumber)} - 1) * {nameof(pageSize)} cannot be larger than int.MaxValue ({int.MaxValue}) due to Entity framework core restriction."
                );
            }
            ulong totalCount = (ulong)await query.LongCountAsync(cancellationToken);
            ImmutableArray<T> results = await query
                .Skip((int)skipLong)
                .Take((int)pageSize)
                .ToImmutableArrayAsync(cancellationToken);
            Page<T> page = new(results, totalCount, pageSize);
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
            ImmutableArray<TEntity>.Builder entitiesBuilder = ImmutableArray.CreateBuilder<TEntity>();
            foreach (TData[] dataChunk in dataSource.Chunk(chunkSize))
            {
                List<TEntity> entityChunk = await query
                    .WhereAny(dataChunk, predicateBuilder)
                    .ToListAsync(cancellationToken);
                entitiesBuilder.AddRange(entityChunk);
            }
            ImmutableArray<TEntity> entities = entitiesBuilder.ToImmutableArray();
            return entities;
        }

        public static async Task<ImmutableArray<TEntity>> WhereAnyAsync<TEntity, TData>(this IQueryable<TEntity> query,
            IEnumerable<TData> dataSource,
            int chunkSize,
            Func<TData, Expression<Func<TEntity, bool>>> predicateBuilder,
            int initialCapacity,
            CancellationToken cancellationToken = default
        )
        {
            ImmutableArray<TEntity>.Builder entitiesBuilder = ImmutableArray.CreateBuilder<TEntity>(initialCapacity);
            foreach (TData[] dataChunk in dataSource.Chunk(chunkSize))
            {
                List<TEntity> entityChunk = await query
                    .WhereAny(dataChunk, predicateBuilder)
                    .ToListAsync(cancellationToken);
                entitiesBuilder.AddRange(entityChunk);
            }
            ImmutableArray<TEntity> entities = entitiesBuilder.ToImmutableArray();
            return entities;
        }
        #endregion
    }
}
