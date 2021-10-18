namespace MASA.Contrib.Data.Uow.EF;

public class UnitOfWork<TDbContext> : IUnitOfWork
    where TDbContext : MasaDbContext
{
    public DbTransaction Transaction
    {
        get
        {
            if (TransactionHasBegun)
            {
                return _context.Database.CurrentTransaction!.GetDbTransaction();
            }
            return _context.Database.BeginTransaction().GetDbTransaction();
        }
    }

    public bool TransactionHasBegun => _context.Database.CurrentTransaction != null;

    public bool DisableRollbackOnFailure { get; set; }

    private readonly DbContext _context;

    private readonly ILogger<UnitOfWork<TDbContext>> _logger;

    public UnitOfWork(TDbContext dbContext, ILogger<UnitOfWork<TDbContext>> logger)
    {
        _context = dbContext;
        _logger = logger;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (!TransactionHasBegun)
        {
            await SaveChangesAsync();
            return;
        }

        try
        {
            await SaveChangesAsync();
            await _context.Database.CommitTransactionAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            if (DisableRollbackOnFailure)
            {
                _logger.LogError(ex, "Failed to commit transaction");
                throw;
            }
            await _context.Database.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, "Failed to commit transaction, rolled back");
        }
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (!TransactionHasBegun)
            throw new NotSupportedException("Transactions are not started and rollback is not supported");
        await _context.Database.RollbackTransactionAsync(cancellationToken);
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
