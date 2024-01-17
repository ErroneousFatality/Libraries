using System.Collections.Immutable;

using AndrejKrizan.DotNet.Repositories;
using AndrejKrizan.EntityFramework.Common.Queries;

using Microsoft.EntityFrameworkCore;

namespace AndrejKrizan.EntityFramework.Common.Repositories;

public class Repository<TEntity> : IRepository<TEntity> 
    where TEntity : class
{
    // Properties
    protected DbContext DbContext { get; }
    protected DbSet<TEntity> DbSet { get; }

    // Constructors
    public Repository(DbContext dbContext)
    {
        DbContext = dbContext;
        DbSet = dbContext.Set<TEntity>();
    }

    // Methods
    public void Insert(TEntity entity)
        => DbSet.Add(entity);

    public void InsertMany(IEnumerable<TEntity> entities)
        => DbSet.AddRange(entities);
    public void InsertMany(params TEntity[] entities)
        => DbSet.AddRange(entities);

    public async Task<ImmutableArray<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        => await DbSet.ToImmutableArrayAsync(cancellationToken);

    public void Delete(TEntity entity)
        => DbSet.Remove(entity);

    public void DeleteMany(IEnumerable<TEntity> entities)
        => DbSet.RemoveRange(entities);

    public void DeleteMany(params TEntity[] entities)
        => DbSet.RemoveRange(entities);
}
