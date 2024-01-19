namespace AndrejKrizan.DotNet.Seeding.Properties.Uniques;
public sealed class SeedUniqueStringProperty<TSeed, TEntity> : SeedUniqueProperty<TSeed, TEntity, string>
{
    // Constructors
    public SeedUniqueStringProperty(
        string description,
        Func<TSeed, string> seedSelector,
        Func<TEntity, string> entitySelector,
        GetValuesAsyncDelegate getValuesAsync,
        StringComparison stringComparison = StringComparison.CurrentCulture
    )
        : this(description, seedSelector, entitySelector, getValuesAsync, StringComparer.FromComparison(stringComparison)) { }

    public SeedUniqueStringProperty(
        string description,
        Func<TSeed, string> seedSelector,
        Func<TEntity, string> entitySelector,
        GetValuesAsyncDelegate getValuesAsync,
        StringComparer stringComparer
    )
        : base(description, seedSelector, entitySelector, getValuesAsync, stringComparer) { }
}
