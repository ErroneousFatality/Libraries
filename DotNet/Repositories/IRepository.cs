using System.Collections.Immutable;

namespace AndrejKrizan.DotNet.Repositories;
public interface IRepository<TEntity>
    where TEntity : class
{
    void Insert(TEntity entity);
    void InsertMany(IEnumerable<TEntity> entities);
    void InsertMany(params TEntity[] entities);

    Task<ImmutableArray<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    void Delete(TEntity entity);
    void DeleteMany(IEnumerable<TEntity> entities);
    void DeleteMany(params TEntity[] entities);

    void Untrack(TEntity entity);
}