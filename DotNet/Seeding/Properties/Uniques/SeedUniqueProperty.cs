using System.Collections.Immutable;

using AndrejKrizan.DotNet.Seeding.Properties.Keys;
using AndrejKrizan.DotNet.Strings;

namespace AndrejKrizan.DotNet.Seeding.Properties.Uniques;
public class SeedUniqueProperty<TEntity, TSeed, TProperty> : SeedProperty<TEntity, TSeed, TProperty>, ISeedUniqueProperty<TEntity, TSeed>
{
    // Constructors

    /// <summary>Uses the <see cref="EqualityComparer{TValue}.Default"/>.</summary>
    public SeedUniqueProperty(
        string description,
        Func<TEntity, TProperty> entitySelector,
        Func<TSeed, TProperty> seedSelector,
        GetValuesAsyncDelegate getValuesAsync
    )
        : this(description, entitySelector, seedSelector, getValuesAsync, EqualityComparer<TProperty>.Default) { }

    public SeedUniqueProperty(
        string description,
        Func<TEntity, TProperty> entitySelector,
        Func<TSeed, TProperty> seedSelector,
        GetValuesAsyncDelegate getValuesAsync,
        IEqualityComparer<TProperty> equalityComparer
    )
        : base(description, entitySelector, seedSelector, equalityComparer, getValuesAsync) { }

    // Methods
    public void RemoveAndLogSeedsWithDuplicateProperties(List<TSeed> seeds, List<string> log, string entityDescription)
        => RemoveAndLogSeedsWithDuplicateValues(seeds, log, entityDescription);

    /// <param name="seeds">Should not contain duplicate properties. Call <see cref="RemoveAndLogSeedsWithDuplicateProperties(List{TSeed}, List{string}, string)"/> first.</param>
    /// <param name="entities">Should not contain duplicate properties.</param>
    /// <exception cref="ArgumentException"></exception>
    public async Task RemoveAndLogSeedsWithExistentPropertiesAsync<TKey>(
        List<TSeed> seeds,
        IEnumerable<TEntity> entities, SeedKey<TEntity, TSeed, TKey> keyProperty,
        List<string> log, string entityDescription,
        CancellationToken cancellationToken = default
    )
        where TKey : notnull
    {
        bool quote = typeof(TProperty) == typeof(string);

        HashSet<TSeed> seedsWithExistentProperties = new(seeds.Count);
        List<TSeed> seedsWithNewProperties = new(seeds.Count);

        // Memory existence check
        Dictionary<TKey, TProperty> entityPropertyDictionary = entities.ToDictionary(
            entity => keyProperty.EntitySelector(entity),
            entity => EntitySelector(entity),
            keyProperty.EqualityComparer
        );

        ImmutableHashSet<TProperty> entityPropertySet = entityPropertyDictionary.Values.ToImmutableHashSet(EqualityComparer);
        if (entityPropertySet.Count < entityPropertyDictionary.Count)
        {
            throw new ArgumentException($"The {entityDescription} entities contain duplicate {Description} values.", nameof(entities));
        }

        foreach (TSeed seed in seeds)
        {
            TProperty property = SeedSelector(seed);
            if (entityPropertySet.Contains(property))
            {
                TKey key = keyProperty.SeedSelector(seed);
                if (!(entityPropertyDictionary.TryGetValue(key, out TProperty? entityProperty) && EqualityComparer.Equals(property, entityProperty)))
                {
                    seedsWithExistentProperties.Add(seed);
                }
                continue;
            }
            seedsWithNewProperties.Add(seed);
        }

        if (seedsWithNewProperties.Count > 0)
        {
            // Database existence check
            IEnumerable<TProperty> newProperties = seedsWithNewProperties.Select(SeedSelector);
            IEnumerable<TProperty> existentProperties = await GetValuesAsync!(newProperties, cancellationToken);
            ImmutableHashSet<TProperty> existentPropertySet = existentProperties.ToImmutableHashSet(EqualityComparer);
            foreach (TSeed seed in seedsWithNewProperties)
            {
                TProperty property = SeedSelector(seed);
                if (existentPropertySet.Contains(property))
                {
                    seedsWithExistentProperties.Add(seed);
                }
            }
        }

        if (seedsWithExistentProperties.Count == 0)
        {
            return;
        }

        seeds.RemoveAll(seedsWithExistentProperties.Contains);
        IEnumerable<TProperty> existentValues = seedsWithExistentProperties.Select(SeedSelector);
        log.Add($"The {entityDescription} seeds contain some {Description} values which already exist: {existentValues.StringJoin(quote: quote)}.");
    }
}
