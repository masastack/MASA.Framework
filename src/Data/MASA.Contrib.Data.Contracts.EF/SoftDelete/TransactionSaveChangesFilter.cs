namespace MASA.Contrib.Data.Contracts.EF.SoftDelete;

public class TransactionSaveChangesFilter : ISaveChangesFilter
{
    private readonly IServiceProvider _serviceProvider;

    public TransactionSaveChangesFilter(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    public void OnExecuting(ChangeTracker changeTracker)
    {
        changeTracker.DetectChanges();
        var unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>();
        if (changeTracker.Entries().Any(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted) && !unitOfWork.TransactionHasBegun)
        {
            var transaction = unitOfWork.Transaction; // Open the transaction
        }
    }
}
