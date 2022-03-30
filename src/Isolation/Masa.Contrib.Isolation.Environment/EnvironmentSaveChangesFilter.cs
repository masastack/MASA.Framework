namespace Masa.Contrib.Isolation.Environment;

public class EnvironmentSaveChangesFilter: ISaveChangesFilter
{
    private readonly IEnvironmentContext _environmentContext;

    public EnvironmentSaveChangesFilter(IEnvironmentContext environmentContext)
    {
        _environmentContext = environmentContext;
    }

    public void OnExecuting(ChangeTracker changeTracker)
    {
        changeTracker.DetectChanges();
        foreach (var entity in changeTracker.Entries().Where(entry => entry.State == EntityState.Added))
        {
            if (entity.Entity is IEnvironment)
            {
                entity.CurrentValues[nameof(IEnvironment.Environment)] = _environmentContext.CurrentEnvironment;
            }
        }
    }
}
