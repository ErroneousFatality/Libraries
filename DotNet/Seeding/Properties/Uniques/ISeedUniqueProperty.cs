using AndrejKrizan.DotNet.Seeding.Properties.Keys;

namespace AndrejKrizan.DotNet.Seeding.Properties.Uniques;
public interface ISeedUniqueProperty<TEntity, TSeed>
{
    void RemoveAndLogSeedsWithDuplicateProperties(List<TSeed> seeds, List<string> log, string entityDescription);

    Task RemoveAndLogSeedsWithExistentPropertiesAsync<TKey>(
        List<TSeed> seeds,
        IEnumerable<TEntity> entities, SeedKey<TEntity, TSeed, TKey> keyProperty,
        List<string> log, string entityDescription,
        CancellationToken cancellationToken = default
    )
        where TKey : notnull;
}
