namespace Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EF;

public class IntegrationEventLogService : IIntegrationEventLogService
{
    private readonly IntegrationEventLogContext _eventLogContext;
    private readonly IServiceProvider _serviceProvider;
    private readonly Logger<IntegrationEventLogService>? _logger;
    private IEnumerable<Type>? _eventTypes;

    public IntegrationEventLogService(
        IntegrationEventLogContext eventLogContext,
        IServiceProvider serviceProvider,
        Logger<IntegrationEventLogService>? logger = null)
    {
        _eventLogContext = eventLogContext;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <summary>
    /// Get messages to retry
    /// </summary>
    /// <param name="retryBatchSize">maximum number of retries per retry</param>
    /// <param name="maxRetryTimes"></param>
    /// <param name="minimumRetryInterval">default: 60s</param>
    /// <returns></returns>
    public async Task<IEnumerable<IntegrationEventLog>> RetrieveEventLogsFailedToPublishAsync(int retryBatchSize = 200, int maxRetryTimes = 10, int minimumRetryInterval = 60)
    {
        //todo: Subsequent acquisition of the current time needs to be uniformly replaced with the unified time method provided by the framework, which is convenient for subsequent uniform replacement to UTC time or other urban time. The default setting here is Utc time.
        var time = DateTime.UtcNow.AddSeconds(-minimumRetryInterval);
        var result = await _eventLogContext.EventLogs
            .Where(e => (e.State == IntegrationEventStates.PublishedFailed || e.State == IntegrationEventStates.InProgress) &&
                e.TimesSent <= maxRetryTimes &&
                e.ModificationTime < time)
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

        if (_eventLogContext.DbContext.Database.CurrentTransaction == null)
            await _eventLogContext.DbContext.Database.UseTransactionAsync(transaction, Guid.NewGuid());

        var eventLogEntry = new IntegrationEventLog(@event, _eventLogContext.DbContext.Database.CurrentTransaction!.TransactionId);
        await _eventLogContext.EventLogs.AddAsync(eventLogEntry);
        await _eventLogContext.DbContext.SaveChangesAsync();

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
                throw new UserFriendlyException($"Failed to modify the state of the local message table to {IntegrationEventStates.Published}, the current State is {eventLog.State}, Id: {eventLog.Id}");
            }
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
                throw new UserFriendlyException($"Failed to modify the state of the local message table to {IntegrationEventStates.InProgress}, the current State is {eventLog.State}, Id: {eventLog.Id}");
            }
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
                throw new UserFriendlyException($"Failed to modify the state of the local message table to {IntegrationEventStates.PublishedFailed}, the current State is {eventLog.State}, Id: {eventLog.Id}");
            }
        });
    }

    public async Task DeleteExpiresAsync(DateTime expiresAt, int batchCount = 1000, CancellationToken token = default)
    {
        var eventLogs = _eventLogContext.EventLogs.Where(e => e.ModificationTime < expiresAt && e.State == IntegrationEventStates.Published)
            .OrderBy(e => e.CreationTime).Take(batchCount);
        _eventLogContext.EventLogs.RemoveRange(eventLogs);
        await _eventLogContext.DbContext.SaveChangesAsync(token);

        if (_eventLogContext.DbContext.ChangeTracker.QueryTrackingBehavior != QueryTrackingBehavior.TrackAll)
        {
            foreach (var log in eventLogs)
                _eventLogContext.DbContext.Entry(log).State = EntityState.Detached;
        }
    }

    private async Task UpdateEventStatus(Guid eventId, IntegrationEventStates status, Action<IntegrationEventLog>? action = null)
    {
        var eventLogEntry = _eventLogContext.EventLogs.AsNoTracking().FirstOrDefault(e => e.EventId == eventId);
        if (eventLogEntry == null)
            throw new ArgumentException(nameof(eventId));

        action?.Invoke(eventLogEntry);

        eventLogEntry.State = status;
        eventLogEntry.ModificationTime = eventLogEntry.GetCurrentTime();

        if (status == IntegrationEventStates.InProgress)
            eventLogEntry.TimesSent++;

        await UpdateAsync(eventLogEntry);
    }

    private void CheckAndDetached(IntegrationEventLog integrationEvent)
    {
        if (_eventLogContext.DbContext.ChangeTracker.QueryTrackingBehavior != QueryTrackingBehavior.TrackAll)
            _eventLogContext.DbContext.Entry(integrationEvent).State = EntityState.Detached;
    }

    /// <summary>
    /// By using SQL statements to handle high concurrency, reduce the dependence on the database
    /// Turning on a transactional operation makes the operation atomic
    /// Retrying the task operation in the background will not open the transaction, so that the task will only be executed once in high concurrency scenarios
    /// </summary>
    /// <param name="eventLogEntry"></param>
    private async Task UpdateAsync(IntegrationEventLog eventLogEntry)
    {
        string eventIdColumn = _eventLogContext.DbContext.GetPropertyName<IntegrationEventLog>(nameof(IntegrationEventLog.EventId));
        string stateColumn = _eventLogContext.DbContext.GetPropertyName<IntegrationEventLog>(nameof(IntegrationEventLog.State));
        string modificationTimeColumn = _eventLogContext.DbContext.GetPropertyName<IntegrationEventLog>(nameof(IntegrationEventLog.ModificationTime));
        string timesSentColumn = _eventLogContext.DbContext.GetPropertyName<IntegrationEventLog>(nameof(IntegrationEventLog.TimesSent));
        string rowVersionColumn = _eventLogContext.DbContext.GetPropertyName<IntegrationEventLog>(nameof(IntegrationEventLog.RowVersion));
        string tableName = _eventLogContext.DbContext.GetTableName<IntegrationEventLog>();
        var newVersion = Guid.NewGuid().ToString();

        string updateSql = $"UPDATE { tableName } set [{ stateColumn }] = {{0}}, [{modificationTimeColumn }] = {{1}}, [{ timesSentColumn }] = {{2}}, [{ rowVersionColumn }] = {{3}} where [{ eventIdColumn }] = {{4}} and [{ rowVersionColumn }] = {{5}};";
        await ExecuteAsync(updateSql, new object[]
        {
            (int)eventLogEntry.State,
            eventLogEntry.ModificationTime,
            eventLogEntry.TimesSent,
            newVersion,
            eventLogEntry.EventId,
            eventLogEntry.RowVersion
        });
    }

    private async Task ExecuteAsync(string updateSql, params object[] parameters)
    {
        var effectRow = await _eventLogContext.DbContext.Database.ExecuteSqlRawAsync(updateSql, parameters);
        if (effectRow == 0)
            throw new UserFriendlyException("Concurrency conflict, update exception");
    }
}
