namespace AndrejKrizan.DotNet.Seeding.Properties.References;
public sealed class SeedReferenceStringProperty<TSeed, TEntity> : SeedReferenceProperty<TSeed, TEntity, string>
{
    // Constructors
    public SeedReferenceStringProperty(
        string description,
        Func<TSeed, string> seedSelector,
        Func<TEntity, string> entitySelector,
        GetValuesAsyncDelegate getValuesAsync,
        StringComparison stringComparison = StringComparison.CurrentCulture
    )
        : this(description, seedSelector, entitySelector, getValuesAsync, StringComparer.FromComparison(stringComparison)) { }

    public SeedReferenceStringProperty(
        string description,
        Func<TSeed, string> seedSelector,
        Func<TEntity, string> entitySelector,
        GetValuesAsyncDelegate getValuesAsync,
        StringComparer stringComparer
    )
        : base(description, seedSelector, entitySelector, getValuesAsync, stringComparer) { }
}
