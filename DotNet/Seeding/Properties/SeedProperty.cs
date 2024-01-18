using AndrejKrizan.DotNet.Collections;
using AndrejKrizan.DotNet.Strings;

namespace AndrejKrizan.DotNet.Seeding.Properties;
public abstract class SeedProperty<TEntity, TSeed, TProperty>
{
    // Properties
    public string Description { get; }
    public Func<TEntity, TProperty> EntitySelector { get; }
    public Func<TSeed, TProperty> SeedSelector { get; }
    public IEqualityComparer<TProperty> EqualityComparer { get; }

    // Protected properties
    public delegate Task<IEnumerable<TProperty>> GetValuesAsyncDelegate(IEnumerable<TProperty> values, CancellationToken cancellationToken = default);
    protected GetValuesAsyncDelegate? GetValuesAsync { get; }

    // Constructors

    /// <summary>Uses the <see cref="EqualityComparer{TValue}.Default"/>.</summary>
    protected SeedProperty(
        string description,
        Func<TEntity, TProperty> entitySelector,
        Func<TSeed, TProperty> seedSelector,
        GetValuesAsyncDelegate? getValuesAsync = null
    )
        : this(description, entitySelector, seedSelector, EqualityComparer<TProperty>.Default, getValuesAsync) { }

    protected SeedProperty(
        string description,
        Func<TEntity, TProperty> entitySelector,
        Func<TSeed, TProperty> seedSelector,
        IEqualityComparer<TProperty> equalityComparer,
        GetValuesAsyncDelegate? getValuesAsync = null
    )
    {
        Description = description;
        EntitySelector = entitySelector;
        SeedSelector = seedSelector;
        EqualityComparer = equalityComparer;
        GetValuesAsync = getValuesAsync;
    }

    // Methods
    protected void RemoveAndLogSeedsWithDuplicateValues(List<TSeed> seeds, List<string> log, string entityDescription)
    {
        IEnumerable<TProperty> values = seeds.Select(SeedSelector);
        HashSet<TProperty> duplicates = values.GetDuplicateSet(EqualityComparer);
        seeds.RemoveAll(seed => duplicates.Contains(SeedSelector(seed)));
        if (duplicates.Count > 0)
        {
            bool quote = typeof(TProperty).Equals(typeof(string));
            log.Add($"There {entityDescription} seeds contain some duplicate {Description} values: {duplicates.StringJoin(quote: quote)}.");
        }
    }
}