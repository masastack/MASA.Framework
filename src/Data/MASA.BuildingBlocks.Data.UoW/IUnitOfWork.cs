namespace MASA.BuildingBlocks.Data.UoW;
public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    DbTransaction Transaction { get; }

    /// <summary>
    /// Whether the transaction has been opened
    /// </summary>
    bool TransactionHasBegun { get; }

    /// <summary>
    /// Whether to use transaction
    /// </summary>
    bool UseTransaction { get; set; }

    /// <summary>
    /// Disable transaction rollback after failure
    /// </summary>
    bool DisableRollbackOnFailure { get; set; }

    EntityState EntityState { get; set; }

    CommitState CommitState { get; set; }

    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    Task CommitAsync(CancellationToken cancellationToken = default);

    Task RollbackAsync(CancellationToken cancellationToken = default);
}
