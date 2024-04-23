namespace AndrejKrizan.DotNet.Seeding.Properties.Uniques;
public sealed class SeedUniqueStringProperty<TEntity, TSeed> : SeedUniqueProperty<TEntity, TSeed, string>
{
    // Constructors
    public SeedUniqueStringProperty(
        string description,
        Func<TEntity, string> entitySelector,
        Func<TSeed, string> seedSelector,
        GetValuesAsyncDelegate getValuesAsync,
        StringComparison stringComparison = StringComparison.CurrentCulture
    )
        : this(description, entitySelector, seedSelector, getValuesAsync, StringComparer.FromComparison(stringComparison)) { }

    public SeedUniqueStringProperty(
        string description,
        Func<TEntity, string> entitySelector,
        Func<TSeed, string> seedSelector,
        GetValuesAsyncDelegate getValuesAsync,
        StringComparer stringComparer
    )
        : base(description, entitySelector, seedSelector, getValuesAsync, stringComparer) { }
}
