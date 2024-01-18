namespace AndrejKrizan.DotNet.Seeding.Properties.Keys;
public sealed class SeedStringKey<TSeed, TEntity> : SeedKey<TSeed, TEntity, string>
{
    // Constructors
    public SeedStringKey(
        string description,
        Func<TSeed, string> seedSelector,
        Func<TEntity, string> entitySelector,
        StringComparison stringComparison = StringComparison.CurrentCulture
    )
        : this(description, seedSelector, entitySelector, StringComparer.FromComparison(stringComparison)) { }


    public SeedStringKey(
        string description,
        Func<TSeed, string> seedSelector,
        Func<TEntity, string> entitySelector,
        StringComparer stringComparer
    )
        : base(description, seedSelector, entitySelector, stringComparer, stringComparer) { }
}
