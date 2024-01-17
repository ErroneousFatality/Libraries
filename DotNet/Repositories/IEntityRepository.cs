using System.Collections.Immutable;

using AndrejKrizan.DotNet.Entities;

namespace AndrejKrizan.DotNet.Repositories;
public interface IEntityRepository<TEntity, TId> : IKeyRepository<TEntity, TId>
    where TEntity : Entity<TId>
    where TId : notnull
{
    Task<ImmutableArray<TId>> GetIdsAsync(IEnumerable<TId> ids, CancellationToken cancellationToken = default);
}