namespace AndrejKrizan.EntityFramework.Common.UnitsOfWork;

public interface IUnitOfWork
{
    // Method definitions
    Task StartTransactionAsync(CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task AbortTransactionAsync(CancellationToken cancellationToken = default);

    // Methods
    public async Task SaveChangesAndCommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        await SaveChangesAsync(cancellationToken);
        await CommitTransactionAsync(cancellationToken);
    }
}