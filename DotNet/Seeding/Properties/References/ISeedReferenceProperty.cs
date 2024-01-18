namespace AndrejKrizan.DotNet.Seeding.Properties.References;
public interface ISeedReferenceProperty<TEntity, TSeed>
{
    // Methods
    Task RemoveAndLogSeedsWithNonExistentReferencesAsync(
        List<TSeed> seeds,
        IEnumerable<TEntity> entities,
        List<string> log, string entityDescription,
        CancellationToken cancellationToken = default
    );
}
