using System.Collections.Immutable;

using AndrejKrizan.DotNet.Collections;
using AndrejKrizan.DotNet.Repositories;
using AndrejKrizan.DotNet.Seeding.Properties.Keys;
using AndrejKrizan.DotNet.Seeding.Properties.References;
using AndrejKrizan.DotNet.Seeding.Properties.Uniques;
using AndrejKrizan.DotNet.Strings;
using AndrejKrizan.DotNet.UnitsOfWork;

namespace AndrejKrizan.DotNet.Seeding;
public static class SeedUtils
{
    // Methods
    public static async Task<SeedResult<TEntity, TKey>> SeedManyAsync<TEntity, TSeed, TKey>(
        string description,
        IEnumerable<TSeed> seeds,
        SeedKey<TEntity, TSeed, TKey> keyProperty,
        IUnitOfWork unitOfWork, IKeyRepository<TEntity, TKey> repository,
        Action<TEntity, TSeed> update, Func<TSeed, TEntity> create,
        IEnumerable<ISeedUniqueProperty<TEntity, TSeed>>? uniqueProperties = null,
        IEnumerable<ISeedReferenceProperty<TEntity, TSeed>>? referenceProperties = null,
        CancellationToken cancellationToken = default
    )
        where TEntity : class
        where TKey : notnull
    {
        SeedResult<TEntity, TKey> result = await SeedManyAsync(
            description,
            seeds,
            keyProperty,
            unitOfWork,
            repository.GetManyAsync, repository.Untrack, repository.InsertMany,
            update, create,
            uniqueProperties,
            referenceProperties,
            cancellationToken
        );
        return result;
    }

    public static async Task<SeedResult<TEntity, TKey>> SeedManyAsync<TEntity, TSeed, TKey>(
        string description,
        IEnumerable<TSeed> seeds,
        SeedKey<TEntity, TSeed, TKey> keyProperty,
        IUnitOfWork unitOfWork,
        GetManyAsync<TEntity, TKey> getManyAsync, Action<TEntity> untrack, Action<IEnumerable<TEntity>> insertMany,
        Action<TEntity, TSeed> update, Func<TSeed, TEntity> create,
        IEnumerable<ISeedUniqueProperty<TEntity, TSeed>>? uniqueProperties = null,
        IEnumerable<ISeedReferenceProperty<TEntity, TSeed>>? existingProperties = null,
        CancellationToken cancellationToken = default
    )
        where TEntity : class
        where TKey : notnull
    {
        List<TSeed> _seeds = new(seeds);
        ImmutableArray<ISeedUniqueProperty<TEntity, TSeed>> _uniqueProperties = uniqueProperties == null ? [] : uniqueProperties.ToImmutableArray();
        ImmutableArray<ISeedReferenceProperty<TEntity, TSeed>> _existingProperties = existingProperties == null ? [] : existingProperties.ToImmutableArray();
        SeedResult<TEntity, TKey> result = await SeedManyImplAsync(
            description, 
            _seeds, 
            keyProperty, 
            unitOfWork,
            getManyAsync,  untrack, insertMany,
            update, create, 
            _uniqueProperties, 
            _existingProperties, 
            cancellationToken
        );
        return result;
    }

    public delegate Task<ImmutableArray<TEntity>> GetManyAsync<TEntity, TKey>(IEnumerable<TKey> keys, CancellationToken cancellationToken = default);
    private static async Task<SeedResult<TEntity, TKey>> SeedManyImplAsync<TEntity, TSeed, TKey>(
        string description,
        List<TSeed> seeds,
        SeedKey<TEntity, TSeed, TKey> keyProperty,
        IUnitOfWork unitOfWork,
        GetManyAsync<TEntity, TKey> getManyAsync, Action<TEntity> untrack, Action<IEnumerable<TEntity>> insertMany,
        Action<TEntity, TSeed> update, Func<TSeed, TEntity> create,
        ImmutableArray<ISeedUniqueProperty<TEntity, TSeed>> uniqueProperties,
        ImmutableArray<ISeedReferenceProperty<TEntity, TSeed>> referenceProperties,
        CancellationToken cancellationToken = default
    )
        where TEntity : class
        where TKey : notnull
    {
        List<string> errors = new(seeds.Count);

        keyProperty.RemoveAndLogSeedsWithDuplicateKeys(seeds, errors, description);
        foreach (ISeedUniqueProperty<TEntity, TSeed> uniqueProperty in uniqueProperties)
        {
            uniqueProperty.RemoveAndLogSeedsWithDuplicateProperties(seeds, errors, description);
        }

        await unitOfWork.StartTransactionAsync(cancellationToken);

        IEnumerable<TKey> keys = seeds.Select(keyProperty.SeedSelector);
        IReadOnlyCollection<TEntity> existingEntities = await getManyAsync(keys, cancellationToken);

        foreach (ISeedUniqueProperty<TEntity, TSeed> uniqueProperty in uniqueProperties)
        {
            await uniqueProperty.RemoveAndLogSeedsWithExistentPropertiesAsync(seeds, existingEntities, keyProperty, errors, description, cancellationToken);
        }

        foreach (ISeedReferenceProperty<TEntity, TSeed> referenceProperty in referenceProperties)
        {
            await referenceProperty.RemoveAndLogSeedsWithNonExistentReferencesAsync(seeds, existingEntities, errors, description, cancellationToken);
        }

        Dictionary<TKey, TEntity> existingEntitiesDict = existingEntities.ToDictionary(keyProperty.EntitySelector, keyProperty.EqualityComparer);
        List<TEntity> updatedEntities = new(seeds.Count);
        List<TEntity> newEntities = new(seeds.Count);
        foreach (TSeed seed in seeds)
        {
            TKey key = keyProperty.SeedSelector(seed);
            if (existingEntitiesDict.TryGetValue(key, out TEntity? entity))
            {
                try
                {
                    update(entity, seed);
                    updatedEntities.Add(entity);
                }
                catch (Exception exception)
                {
                    errors.Add($"Failed to update {description} (Key = \'{key}\'): {exception.Message}.");
                    untrack(entity);
                    continue;
                }
            }
            else
            {
                try
                {
                    entity = create(seed);
                    newEntities.Add(entity);
                }
                catch (Exception exception)
                {
                    errors.Add($"Failed to create {description} (Key = \'{key}\'): {exception.Message}.");
                    continue;
                }
            }

        }

        insertMany(newEntities);
        await unitOfWork.SaveChangesAndCommitTransactionAsync(cancellationToken);

        ImmutableDictionary<TKey, TEntity> entities = updatedEntities.Concat(newEntities)
            .OrderBy(keyProperty.EntitySelector, keyProperty.Comparer)
            .ToImmutableDictionary(keyProperty.EntitySelector, keyProperty.EqualityComparer);

        return new SeedResult<TEntity, TKey>
        {
            Entities = entities,
            Errors = errors.ToImmutableArray()
        };
    }

    #region RemoveAndLogDuplicatesBy
    public static void RemoveAndLogDuplicatesBy<T, TValue>(
        List<T> objects, Func<T, TValue> getValue,
        List<string> log, string valuesDescription,
        IEqualityComparer<TValue> comparer
    )
    {
        IEnumerable<TValue> values = objects.Select(getValue);
        HashSet<TValue> duplicates = values.GetDuplicateSet(comparer);
        objects.RemoveAll(obj =>
        {
            TValue value = getValue(obj);
            bool hasDuplicate = duplicates.Contains(value);
            return hasDuplicate;
        });
        if (duplicates.Count > 0)
        {
            log.Add($"There are duplicate {valuesDescription}: {duplicates.StringJoin()}.");
        }
    }

    /// <summary>Uses the <see cref="EqualityComparer{TValue}.Default"/>.</summary>
    public static void RemoveAndLogDuplicatesBy<T, TValue>(
        List<T> objects, Func<T, TValue> getValue,
        List<string> log, string valuesDescription
    )
        => RemoveAndLogDuplicatesBy(
            objects, getValue,
            log, valuesDescription,
            EqualityComparer<TValue>.Default
        );


    public static void RemoveAndLogDuplicatesBy<T>(
        List<T> objects, Func<T, string> getValue,
        List<string> log, string valuesDescription,
        StringComparer comparer
    )
    {
        IEnumerable<string> values = objects.Select(getValue);
        HashSet<string> duplicates = values.GetDuplicateSet(comparer);
        objects.RemoveAll(obj =>
        {
            string value = getValue(obj);
            bool hasDuplicate = duplicates.Contains(value);
            return hasDuplicate;
        });
        if (duplicates.Count > 0)
        {
            log.Add($"There are duplicate {valuesDescription}: {duplicates.StringJoin(quote: true)}.");
        }
    }

    public static void RemoveAndLogDuplicatesBy<T>(
        List<T> objects, Func<T, string> getValue,
        List<string> log, string valuesDescription,
        StringComparison comparison = StringComparison.CurrentCulture
    )
        => RemoveAndLogDuplicatesBy(
            objects, getValue,
            log, valuesDescription,
            StringComparer.FromComparison(comparison)
        );
    #endregion

    #region RemoveAndLogNonExistent
    public static async Task RemoveAndLogNonExistentAsync<T, TValue>(
        List<T> objects, Func<T, TValue> getValue,
        Func<HashSet<TValue>, CancellationToken, Task<IEnumerable<TValue>>> getExistingValuesAsync,
        List<string> log, string valuesDescription,
        IEqualityComparer<TValue> comparer,
        CancellationToken cancellationToken = default
    )
    {
        HashSet<TValue> values = objects.Select(getValue).ToHashSet(comparer);
        IEnumerable<TValue> existingValues = await getExistingValuesAsync(values, cancellationToken);
        values.ExceptWith(existingValues);
        if (values.Count > 0)
        {
            objects.RemoveAll(seed => values.Contains(getValue(seed)));
            log.Add($"There are some {valuesDescription} which do not exist: {values.StringJoin()}.");
        }
    }

    /// <summary>Uses the <see cref="EqualityComparer{TValue}.Default"/>.</summary>
    public static Task RemoveAndLogNonExistentAsync<T, TValue>(
        List<T> objects, Func<T, TValue> getValue,
        Func<HashSet<TValue>, CancellationToken, Task<IEnumerable<TValue>>> getExistingValuesAsync,
        List<string> log, string valuesDescription,
        CancellationToken cancellationToken = default
    )
        => RemoveAndLogNonExistentAsync(
            objects, getValue,
            getExistingValuesAsync,
            log, valuesDescription,
            EqualityComparer<TValue>.Default,
            cancellationToken
        );


    public static async Task RemoveAndLogNonExistentAsync<T>(
        List<T> objects, Func<T, string> getValue,
        Func<HashSet<string>, CancellationToken, Task<IEnumerable<string>>> getExistingValuesAsync,
        List<string> log, string valuesDescription,
        StringComparer comparer,
        CancellationToken cancellationToken = default
    )
    {
        HashSet<string> values = objects.Select(getValue).ToHashSet(comparer);
        IEnumerable<string> existingValues = await getExistingValuesAsync(values, cancellationToken);
        values.ExceptWith(existingValues);
        if (values.Count > 0)
        {
            objects.RemoveAll(seed => values.Contains(getValue(seed)));
            log.Add($"There are some {valuesDescription} which do not exist: {values.StringJoin(quote: true)}.");
        }
    }

    public static Task RemoveAndLogNonExistentAsync<T>(
        List<T> objects, Func<T, string> getValue,
        Func<HashSet<string>, CancellationToken, Task<IEnumerable<string>>> getExistingValuesAsync,
        List<string> log, string valuesDescription,
        StringComparison comparison = StringComparison.CurrentCulture,
        CancellationToken cancellationToken = default
    )
        => RemoveAndLogNonExistentAsync(
            objects, getValue,
            getExistingValuesAsync,
            log, valuesDescription,
            StringComparer.FromComparison(comparison),
            cancellationToken
        );
    #endregion
}