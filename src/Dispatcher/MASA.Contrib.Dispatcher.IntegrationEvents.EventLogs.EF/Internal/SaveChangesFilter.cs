namespace MASA.Contrib.Dispatcher.IntegrationEvents.EventLogs.EF.Internal;

internal abstract class SaveChangesFilter : ISaveChangesFilter
{
    public abstract void OnExecuting(ChangeTracker changeTracker);
}
