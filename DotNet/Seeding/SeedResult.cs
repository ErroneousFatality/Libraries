using System.Collections.Immutable;

namespace AndrejKrizan.DotNet.Seeding;
public readonly struct SeedResult<TEntity, TKey>
    where TKey : notnull
{
    public required readonly ImmutableArray<string> Errors { get; init; }
    public required readonly ImmutableDictionary<TKey, TEntity> Entities { get; init; }
}
