namespace MASA.Contrib.Dispatcher.IntegrationEvents.EventLogs.EF;

public class IntegrationEventLogService : IIntegrationEventLogService
{
    private readonly IntegrationEventLogContext _eventLogContext;
    private readonly IServiceProvider _serviceProvider;
    private IEnumerable<Type>? _eventTypes;
    private readonly RetryStrategyOptions _retryStrategyOptions;

    public IntegrationEventLogService(IntegrationEventLogContext eventLogContext, IServiceProvider serviceProvider,RetryStrategyOptions retryStrategyOptions)
    {
        _eventLogContext = eventLogContext;
        _serviceProvider = serviceProvider;
        _retryStrategyOptions = retryStrategyOptions;
    }

    /// <summary>
    /// Get messages to retry
    /// </summary>
    /// <param name="retryDepth">The size of a single event to be retried</param>
    /// <returns></returns>
    public virtual async Task<IEnumerable<IntegrationEventLog>> RetrieveEventLogsFailedToPublishAsync(int retryDepth)
    {
        var result = await _eventLogContext.EventLogs
            .Where(e => e.State == IntegrationEventStates.PublishedFailed && e.TimesSent <= _retryStrategyOptions.MaxRetryTimes)//todo: Need to add NextRetryTime condition
            .OrderBy(o => o.CreationTime)
            .Take(retryDepth)
            .ToListAsync();

        if (result.Any())
        {
            _eventTypes ??= _serviceProvider.GetRequiredService<IIntegrationEventBus>().GetAllEventTypes()
                .Where(type => typeof(IIntegrationEvent).IsAssignableFrom(type));

            return result.OrderBy(o => o.CreationTime)
                .Select(e => e.DeserializeJsonContent(_eventTypes.First(t => t.Name == e.EventTypeShortName)));
        }

        return new List<IntegrationEventLog>();
    }

    public async Task<IEnumerable<IntegrationEventLog>> RetrieveEventLogsPendingToPublishAsync(Guid transactionId)
    {
        var result = await _eventLogContext.EventLogs
            .Where(e => e.TransactionId == transactionId && e.State == IntegrationEventStates.NotPublished)
            .OrderBy(o => o.CreationTime)
            .ToListAsync();

        if (result.Any())
        {
            _eventTypes ??= _serviceProvider.GetRequiredService<IIntegrationEventBus>().GetAllEventTypes()
                .Where(type => typeof(IIntegrationEvent).IsAssignableFrom(type));

            return result
                .Select(e => e.DeserializeJsonContent(_eventTypes.First(t => t.Name == e.EventTypeShortName)));
        }

        return new List<IntegrationEventLog>();
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
        return UpdateEventStatus(eventId, IntegrationEventStates.Published);
    }

    public Task MarkEventAsInProgressAsync(Guid eventId)
    {
        return UpdateEventStatus(eventId, IntegrationEventStates.InProgress);
    }

    public Task MarkEventAsFailedAsync(Guid eventId)
    {
        return UpdateEventStatus(eventId, IntegrationEventStates.PublishedFailed);//Need to mark the next retry time and add a retry record
    }

    private async Task UpdateEventStatus(Guid eventId, IntegrationEventStates status)
    {
        var eventLogEntry = _eventLogContext.EventLogs.FirstOrDefault(e => e.EventId == eventId);
        if (eventLogEntry == null)
            throw new ArgumentException(nameof(eventId));

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
