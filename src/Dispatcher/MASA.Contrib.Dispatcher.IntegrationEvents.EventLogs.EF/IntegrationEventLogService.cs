namespace MASA.Contrib.Dispatcher.IntegrationEvents.EventLogs.EF;

public class IntegrationEventLogService : IIntegrationEventLogService
{
    private readonly IntegrationEventLogContext _eventLogContext;
    private readonly IServiceProvider _serviceProvider;
    private readonly Logger<IntegrationEventLogService>? _logger;
    private IEnumerable<Type>? _eventTypes;

    public IntegrationEventLogService(
        IntegrationEventLogContext eventLogContext,
        IServiceProvider serviceProvider,
        Logger<IntegrationEventLogService>? logger)
    {
        _eventLogContext = eventLogContext;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <summary>
    /// Get messages to retry
    /// </summary>
    /// <param name="retryBatchSize">The size of a single event to be retried</param>
    /// <param name="maxRetryTimes"></param>
    /// <returns></returns>
    public async Task<IEnumerable<IntegrationEventLog>> RetrieveEventLogsFailedToPublishAsync(int retryBatchSize = 200,
        int maxRetryTimes = 10)
    {
        var result = await _eventLogContext.EventLogs
            .Where(e => (e.State == IntegrationEventStates.PublishedFailed || e.State == IntegrationEventStates.InProgress) &&
                e.TimesSent <= maxRetryTimes)
            .OrderBy(o => o.CreationTime)
            .Take(retryBatchSize)
            .ToListAsync();

        if (result.Any())
        {
            _eventTypes ??= _serviceProvider.GetRequiredService<IIntegrationEventBus>().GetAllEventTypes()
                .Where(type => typeof(IIntegrationEvent).IsAssignableFrom(type));

            return result.OrderBy(o => o.CreationTime)
                .Select(e => e.DeserializeJsonContent(_eventTypes.First(t => t.Name == e.EventTypeShortName)));
        }

        return result;
    }

    public async Task SaveEventAsync(IIntegrationEvent @event, DbTransaction transaction)
    {
        if (transaction == null)
            throw new ArgumentNullException(nameof(transaction));

        if (_eventLogContext.Database.CurrentTransaction == null)
            await _eventLogContext.Database.UseTransactionAsync(transaction, Guid.NewGuid());

        var eventLogEntry = new IntegrationEventLog(@event, _eventLogContext.Database.CurrentTransaction!.TransactionId);
        await _eventLogContext.EventLogs.AddAsync(eventLogEntry);
        await _eventLogContext.SaveChangesAsync();

        CheckAndDetached(eventLogEntry);
    }

    public Task MarkEventAsPublishedAsync(Guid eventId)
    {
        return UpdateEventStatus(eventId, IntegrationEventStates.Published, eventLog =>
        {
            if (eventLog.State != IntegrationEventStates.InProgress)
            {
                _logger?.LogWarning(
                    "Failed to modify the state of the local message table to {OptState}, the current State is {State}, Id: {Id}",
                    IntegrationEventStates.Published, eventLog.State, eventLog.Id);
                return true;
            }
            return false;
        });
    }

    public Task MarkEventAsInProgressAsync(Guid eventId)
    {
        return UpdateEventStatus(eventId, IntegrationEventStates.InProgress, eventLog =>
        {
            if (eventLog.State != IntegrationEventStates.NotPublished && eventLog.State != IntegrationEventStates.PublishedFailed)
            {
                _logger?.LogWarning(
                    "Failed to modify the state of the local message table to {OptState}, the current State is {State}, Id: {Id}",
                    IntegrationEventStates.InProgress, eventLog.State, eventLog.Id);
                return true;
            }
            return false;
        });
    }

    public Task MarkEventAsFailedAsync(Guid eventId)
    {
        return UpdateEventStatus(eventId, IntegrationEventStates.PublishedFailed, eventLog =>
        {
            if (eventLog.State != IntegrationEventStates.InProgress)
            {
                _logger?.LogWarning(
                    "Failed to modify the state of the local message table to {OptState}, the current State is {State}, Id: {Id}",
                    IntegrationEventStates.PublishedFailed, eventLog.State, eventLog.Id);
                return true;
            }
            return false;
        });
    }

    public async Task DeleteExpiresAsync(DateTime expiresAt, int batchCount = 1000, CancellationToken token = default)
    {
        var eventLogs = _eventLogContext.EventLogs.Where(e => e.CreationTime < expiresAt && e.State == IntegrationEventStates.Published)
            .OrderBy(e => e.CreationTime).Take(batchCount);
        _eventLogContext.EventLogs.RemoveRange(eventLogs);
        await _eventLogContext.SaveChangesAsync(token);

        if (_eventLogContext.ChangeTracker.QueryTrackingBehavior != QueryTrackingBehavior.TrackAll)
        {
            foreach (var log in eventLogs)
            {
                _eventLogContext.Entry(log).State = EntityState.Detached;
            }
        }
    }

    private async Task UpdateEventStatus(Guid eventId, IntegrationEventStates status, Func<IntegrationEventLog, bool>? action = null)
    {
        var eventLogEntry = _eventLogContext.EventLogs.FirstOrDefault(e => e.EventId == eventId);
        if (eventLogEntry == null)
            throw new ArgumentException(nameof(eventId));

        var isSkip = action?.Invoke(eventLogEntry) ?? false;
        if (isSkip)
            return;

        eventLogEntry.State = status;

        if (status == IntegrationEventStates.InProgress)
            eventLogEntry.TimesSent++;

        _eventLogContext.EventLogs.Update(eventLogEntry);
        await _eventLogContext.SaveChangesAsync();

        CheckAndDetached(eventLogEntry);
    }

    private void CheckAndDetached(IntegrationEventLog integrationEvent)
    {
        if (_eventLogContext.ChangeTracker.QueryTrackingBehavior != QueryTrackingBehavior.TrackAll)
        {
            _eventLogContext.Entry(integrationEvent).State = EntityState.Detached;
        }
    }
}
