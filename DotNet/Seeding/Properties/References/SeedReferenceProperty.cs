using AndrejKrizan.DotNet.Strings;

namespace AndrejKrizan.DotNet.Seeding.Properties.References;
public class SeedReferenceProperty<TEntity, TSeed, TProperty> : SeedProperty<TEntity, TSeed, TProperty>, ISeedReferenceProperty<TEntity, TSeed>
{
    // Constructors

    /// <summary>Uses the <see cref="EqualityComparer{TValue}.Default"/>.</summary>
    public SeedReferenceProperty(
        string description,
        Func<TEntity, TProperty> entitySelector,
        Func<TSeed, TProperty> seedSelector,
        GetValuesAsyncDelegate getValuesAsync
    )
        : this(description, entitySelector, seedSelector, getValuesAsync, EqualityComparer<TProperty>.Default) { }

    public SeedReferenceProperty(
        string description,
        Func<TEntity, TProperty> entitySelector,
        Func<TSeed, TProperty> seedSelector,
        GetValuesAsyncDelegate getValuesAsync,
        IEqualityComparer<TProperty> equalityComparer
    )
        : base(description, entitySelector, seedSelector, equalityComparer, getValuesAsync) { }

    // Methods
    public async Task RemoveAndLogSeedsWithNonExistentReferencesAsync(
        List<TSeed> seeds,
        IEnumerable<TEntity> entities,
        List<string> log, string entityDescription,
        CancellationToken cancellationToken = default
    )
    {
        HashSet<TProperty> nonExistentReferences = seeds.Select(SeedSelector).ToHashSet(EqualityComparer);

        // Memory existence check
        IEnumerable<TProperty> existentReferences = entities.Select(EntitySelector);
        nonExistentReferences.ExceptWith(existentReferences);
        if (nonExistentReferences.Count == 0)
        {
            return;
        }

        // Database existence check
        existentReferences = await GetValuesAsync!(nonExistentReferences, cancellationToken);
        nonExistentReferences.ExceptWith(existentReferences);
        if (nonExistentReferences.Count == 0)
        {
            return;
        }

        seeds.RemoveAll(seed => nonExistentReferences.Contains(SeedSelector(seed)));
        bool quote = typeof(TProperty) == typeof(string);
        log.Add($"The {entityDescription} seeds contain some {Description} references which do not exist: {nonExistentReferences.StringJoin(quote: quote)}.");
    }
}
