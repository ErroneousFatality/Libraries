using Microsoft.EntityFrameworkCore;

namespace AndrejKrizan.EntityFramework.Common.Repositories
{
    public abstract class UnitOfWork
    {
        // Properties
        private DbContext Context { get; }

        // Constructors
        protected UnitOfWork(DbContext dbContext)
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

        public async Task SaveChangesAndCommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            await SaveChangesAsync(cancellationToken);
            await CommitTransactionAsync(cancellationToken);
        }
    }
}