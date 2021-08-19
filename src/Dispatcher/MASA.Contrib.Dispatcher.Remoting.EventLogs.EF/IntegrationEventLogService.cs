namespace MASA.Contrib.Dispatcher.Remoting.EventLogs.EF;
public class IntegrationEventLogService : IIntegrationEventLogService
{
    private readonly IntegrationEventLogContext _eventLogContext;
    private static readonly List<Type> _eventTypes;

    static IntegrationEventLogService()
    {
        _eventTypes = Assembly.Load(Assembly.GetEntryAssembly()!.FullName!)
                .GetTypes()
                .Where(t => t.Name.EndsWith(nameof(IntegrationEvent)))
                .ToList();
    }

    public IntegrationEventLogService(DbContextOptions<IntegrationEventLogContext> options)
    {
        if (options is null) throw new ArgumentNullException(nameof(options));

        _eventLogContext = new IntegrationEventLogContext(options);
    }

    public async Task<IEnumerable<IntegrationEventLog>> RetrieveEventLogsPendingToPublishAsync(Guid transactionId)
    {
        var result = await _eventLogContext.EventLogs
            .Where(e => e.TransactionId == transactionId && e.State == IntegrationEventStates.NotPublished).ToListAsync();

        if (result.Any())
        {
            return result.OrderBy(o => o.CreationTime)
                .Select(e => e.DeserializeJsonContent(_eventTypes.First(t => t.Name == e.EventTypeShortName)));
        }

        return new List<IntegrationEventLog>();
    }

    public Task SaveEventAsync(IntegrationEvent @event, DbTransaction transaction)
    {
        if (transaction == null) throw new ArgumentNullException(nameof(transaction));

        // todo transaction Id
        var eventLogEntry = new IntegrationEventLog(@event, Guid.NewGuid());

        _eventLogContext.Database.UseTransaction(transaction);
        _eventLogContext.EventLogs.Add(eventLogEntry);

        return _eventLogContext.SaveChangesAsync();
    }

    public Task MarkEventAsPublishedAsync(Guid eventId)
    {
        return UpdateEventStatus(eventId, IntegrationEventStates.Published);
    }

    public Task MarkEventAsInProgressAsync(Guid eventId)
    {
        return UpdateEventStatus(eventId, IntegrationEventStates.InProgress);
    }

    public Task MarkEventAsFailedAsync(Guid eventId)
    {
        return UpdateEventStatus(eventId, IntegrationEventStates.PublishedFailed);
    }

    private Task UpdateEventStatus(Guid eventId, IntegrationEventStates status)
    {
        var eventLogEntry = _eventLogContext.EventLogs.Single(e => e.Id == eventId);
        eventLogEntry.State = status;

        if (status == IntegrationEventStates.InProgress)
            eventLogEntry.TimesSent++;

        _eventLogContext.EventLogs.Update(eventLogEntry);

        return _eventLogContext.SaveChangesAsync();
    }
}