namespace MASA.Contrib.Data.Contracts.EF.SoftDelete;

public class SoftDeleteSaveChangesFilter : ISaveChangesFilter
{
    public void OnExecuting(ChangeTracker changeTracker)
    {
        foreach (var entity in changeTracker.Entries().Where(e => e.State == EntityState.Deleted))
        {
            if (entity.Entity is ISoftDelete)
            {
                entity.State = EntityState.Modified;
                entity.CurrentValues[nameof(ISoftDelete.IsDeleted)] = true;
            }
        }
    }
}
