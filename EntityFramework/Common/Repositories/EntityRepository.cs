using AndrejKrizan.DotNet.Entities;

using Microsoft.EntityFrameworkCore;

namespace AndrejKrizan.EntityFramework.Common.Repositories;

public class EntityRepository<TEntity, TId> : KeyRepository<TEntity, TId>
    where TEntity : Entity<TId>
    where TId : struct
{
    // Constructors
    public EntityRepository(DbContext dbContext)
        : base(dbContext, key: entity => entity.Id) { }
}
