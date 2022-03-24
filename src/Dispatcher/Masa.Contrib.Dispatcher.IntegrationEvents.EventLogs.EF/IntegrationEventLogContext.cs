namespace Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EF;

public class IntegrationEventLogContext
{
    public readonly DbContext DbContext;

    public IntegrationEventLogContext(DbContext dbContext) => DbContext = dbContext;

    public DbSet<IntegrationEventLog> EventLogs => DbContext.Set<IntegrationEventLog>();
}
