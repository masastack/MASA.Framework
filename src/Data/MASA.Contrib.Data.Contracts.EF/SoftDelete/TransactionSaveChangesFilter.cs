namespace MASA.Contrib.Data.Contracts.EF.SoftDelete
{
    public class TransactionSaveChangesFilter : ISaveChangesFilter
    {
        private readonly IUnitOfWork _unitOfWork;

        public TransactionSaveChangesFilter(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public void OnExecuting(ChangeTracker changeTracker)
        {
            changeTracker.DetectChanges();
            if (changeTracker.Entries().Any(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted) && !_unitOfWork.TransactionHasBegun)
            {
                var transaction = _unitOfWork.Transaction; // Open the transaction
            }
        }
    }
}
