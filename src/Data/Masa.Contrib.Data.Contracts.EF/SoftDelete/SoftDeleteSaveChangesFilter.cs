namespace Masa.Contrib.Data.Contracts.EF.SoftDelete;

public class SoftDeleteSaveChangesFilter : ISaveChangesFilter
{
    public void OnExecuting(ChangeTracker changeTracker)
    {
        changeTracker.DetectChanges();
        foreach (var entity in changeTracker.Entries().Where(e => e.State == Microsoft.EntityFrameworkCore.EntityState.Deleted))
        {
            if (entity.Entity is ISoftDelete)
            {
                entity.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                entity.CurrentValues[nameof(ISoftDelete.IsDeleted)] = true;
            }
        }
    }
}
