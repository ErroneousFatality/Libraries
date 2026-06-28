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

    void Untrack(TEntity entity);

    [Obsolete(message: $"Use the {nameof(DeleteMany)} with an inline initialized collection instead. E.g.: [1, 2, 3]", error: true)]
    void DeleteMany(params TEntity[] entities);
}