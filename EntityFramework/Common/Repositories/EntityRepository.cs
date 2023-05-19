using System.Linq.Expressions;

using AndrejKrizan.DotNet.Entities;
using AndrejKrizan.EntityFramework.Common.Repositories.Key;
using Microsoft.EntityFrameworkCore;

namespace AndrejKrizan.EntityFramework.Common.Repositories;

public class EntityRepository<TEntity, TId> : KeyRepository<TEntity, TId>
    where TEntity : Entity<TId>
    where TId : struct
{
    // Constructors
    public EntityRepository(DbContext dbContext)
        : base(dbContext) { }

    protected override Expression<Func<TEntity, TId>> KeyLambda()
        => entity => entity.Id;
}
