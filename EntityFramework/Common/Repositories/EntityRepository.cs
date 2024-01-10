using System.Collections.Immutable;
using System.Linq.Expressions;

using AndrejKrizan.DotNet.Entities;

using Microsoft.EntityFrameworkCore;

namespace AndrejKrizan.EntityFramework.Common.Repositories;
public class EntityRepository<TEntity, TId> : KeyRepository<TEntity, TId>
    where TEntity : Entity<TId>
    where TId : notnull
{
    // Protected properties
    protected override Expression<Func<TEntity, TId>> KeySelector
        => entity => entity.Id;

    // Constructors
    public EntityRepository(DbContext dbContext)
        : base(dbContext) { }

    // Methods
    public Task<ImmutableArray<TId>> GetIdsAsync(IEnumerable<TId> ids, CancellationToken cancellationToken = default)
        => GetKeysAsync(ids, cancellationToken);
}
