namespace Masa.Contrib.Data.UoW.EF;

public class UnitOfWork<TDbContext> : IUnitOfWork where TDbContext : MasaDbContext
{
    public IServiceProvider ServiceProvider { get; }

    private DbContext? _context;

    protected DbContext Context => _context ??= ServiceProvider.GetRequiredService<TDbContext>();

    public DbTransaction Transaction
    {
        get
        {
            if (!UseTransaction)
                throw new NotSupportedException("Doesn't support transaction opening");

            if (TransactionHasBegun)
                return Context.Database.CurrentTransaction!.GetDbTransaction();

            return Context.Database.BeginTransaction().GetDbTransaction();
        }
    }

    public bool TransactionHasBegun => Context.Database.CurrentTransaction != null;

    public bool DisableRollbackOnFailure { get; set; }

    public EntityState EntityState { get; set; } = EntityState.UnChanged;

    public CommitState CommitState { get; set; } = CommitState.Commited;

    public bool UseTransaction { get; set; } = true;

    public UnitOfWork(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await Context.SaveChangesAsync(cancellationToken);
        EntityState = EntityState.UnChanged;
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (!UseTransaction || !TransactionHasBegun)
            throw new NotSupportedException("Transaction not opened");

        await Context.Database.CommitTransactionAsync(cancellationToken);
        CommitState = CommitState.Commited;
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (!UseTransaction || !TransactionHasBegun)
            throw new NotSupportedException("Transactions are not opened and rollback is not supported");

        await Context.Database.RollbackTransactionAsync(cancellationToken);
    }

    public async ValueTask DisposeAsync() => await (_context?.DisposeAsync() ?? ValueTask.CompletedTask);

    public void Dispose() => _context?.Dispose();
}
