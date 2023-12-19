using AndrejKrizan.DotNet.UnitsOfWork;

using Microsoft.EntityFrameworkCore;

namespace AndrejKrizan.EntityFramework.Common.UnitsOfWork;

public class UnitOfWork : IUnitOfWork
{
    // Properties
    private readonly DbContext Context;

    // Constructors
    public UnitOfWork(DbContext dbContext)
    {
        Context = dbContext;
    }

    // Methods
    public async Task StartTransactionAsync(CancellationToken cancellationToken = default)
        => await Context.Database.BeginTransactionAsync(cancellationToken);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await Context.SaveChangesAsync(cancellationToken);

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        => await Context.Database.CommitTransactionAsync(cancellationToken);

    public async Task AbortTransactionAsync(CancellationToken cancellationToken = default)
        => await Context.Database.RollbackTransactionAsync(cancellationToken);
}