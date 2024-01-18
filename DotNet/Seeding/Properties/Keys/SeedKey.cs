namespace AndrejKrizan.DotNet.Seeding.Properties.Keys;
public class SeedKey<TEntity, TSeed, TKey> : SeedProperty<TEntity, TSeed, TKey>
{
    // Properties
    public IComparer<TKey> Comparer { get; }

    // Constructors

    /// <summary>Uses the <see cref="EqualityComparer{TValue}.Default"/> and <see cref="Comparer{TValue}.Default"/>.</summary>
    public SeedKey(
        string description,
        Func<TEntity, TKey> entitySelector,
        Func<TSeed, TKey> seedSelector
    )
        : this(description, entitySelector, seedSelector, EqualityComparer<TKey>.Default, Comparer<TKey>.Default) { }

    /// <summary>Uses the <see cref="Comparer{TValue}.Default"/>.</summary>
    public SeedKey(
        string description,
        Func<TEntity, TKey> entitySelector,
        Func<TSeed, TKey> seedSelector,
        IEqualityComparer<TKey> equalityComparer
    )
        : this(description, entitySelector, seedSelector, equalityComparer, Comparer<TKey>.Default) { }

    /// <summary>Uses the <see cref="EqualityComparer{TValue}.Default"/>.</summary>
    public SeedKey(
        string description,
        Func<TEntity, TKey> entitySelector,
        Func<TSeed, TKey> seedSelector,
        IComparer<TKey> comparer
    )
        : this(description, entitySelector, seedSelector, EqualityComparer<TKey>.Default, comparer) { }

    public SeedKey(
        string description,
        Func<TEntity, TKey> entitySelector,
        Func<TSeed, TKey> seedSelector,
        IEqualityComparer<TKey> equalityComparer,
        IComparer<TKey> comparer
    )
        : base(description, entitySelector, seedSelector, equalityComparer)
    {
        Comparer = comparer;
    }

    // Methods
    public void RemoveAndLogSeedsWithDuplicateKeys(List<TSeed> seeds, List<string> log, string entityDescription)
        => RemoveAndLogSeedsWithDuplicateValues(seeds, log, entityDescription);
}
