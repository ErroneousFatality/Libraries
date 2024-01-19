using System.Collections.Immutable;

namespace AndrejKrizan.DotNet.Seeding;
public readonly struct SeedResult<TEntity>
{
    public required readonly ImmutableArray<string> Errors { get; init; }
    public required readonly ImmutableArray<TEntity> Entities { get; init; }
}
