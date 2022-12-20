// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EFCore;

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
    /// <param name="minimumRetryInterval">Minimum retry interval (unit: s)</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IEnumerable<IntegrationEventLog>> RetrieveEventLogsFailedToPublishAsync(
        int retryBatchSize,
        int maxRetryTimes,
        int minimumRetryInterval,
        CancellationToken cancellationToken = default)
    {
        var time = DateTime.UtcNow.AddSeconds(-minimumRetryInterval);
        var result = await _eventLogContext.EventLogs
            .Where(e => (e.State == IntegrationEventStates.PublishedFailed || e.State == IntegrationEventStates.InProgress) &&
                e.TimesSent <= maxRetryTimes &&
                e.ModificationTime < time)
            .OrderBy(e => e.CreationTime)
            .Take(retryBatchSize)
            .ToListAsync(cancellationToken);

        if (result.Any())
        {
            _eventTypes ??= _serviceProvider.GetRequiredService<IIntegrationEventBus>().GetAllEventTypes()
                .Where(type => typeof(IIntegrationEvent).IsAssignableFrom(type));

            return result.OrderBy(e => e.CreationTime)
                .Select(e => e.DeserializeJsonContent(_eventTypes.First(t => t.Name == e.EventTypeShortName)));
        }

        return result;
    }

    /// <summary>
    /// Retrieve pending messages
    /// </summary>
    /// <param name="batchSize">The maximum number of messages retrieved each time</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IEnumerable<IntegrationEventLog>> RetrieveEventLogsPendingToPublishAsync(
        int batchSize,
        CancellationToken cancellationToken = default)
    {
        var result = await _eventLogContext.EventLogs
            .Where(e => e.State == IntegrationEventStates.NotPublished)
            .OrderBy(e => e.CreationTime)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
        if (result.Any())
        {
            _eventTypes ??= _serviceProvider.GetRequiredService<IIntegrationEventBus>().GetAllEventTypes()
                .Where(type => typeof(IIntegrationEvent).IsAssignableFrom(type));

            return result.OrderBy(e => e.CreationTime)
                .Select(e => e.DeserializeJsonContent(_eventTypes.First(t => t.Name == e.EventTypeShortName)));
        }

        return result;
    }

    public async Task SaveEventAsync(IIntegrationEvent @event, DbTransaction transaction, CancellationToken cancellationToken = default)
    {
        MasaArgumentException.ThrowIfNull(transaction);

        if (_eventLogContext.DbContext.Database.CurrentTransaction == null)
            await _eventLogContext.DbContext.Database.UseTransactionAsync(transaction, Guid.NewGuid(),
                cancellationToken: cancellationToken);

        var eventLogEntry = new IntegrationEventLog(@event, _eventLogContext.DbContext.Database.CurrentTransaction!.TransactionId);
        await _eventLogContext.EventLogs.AddAsync(eventLogEntry, cancellationToken);
        await _eventLogContext.DbContext.SaveChangesAsync(cancellationToken);

        CheckAndDetached(eventLogEntry);
    }

    public Task MarkEventAsPublishedAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        return UpdateEventStatus(eventId, IntegrationEventStates.Published, eventLog =>
        {
            if (eventLog.State != IntegrationEventStates.InProgress)
            {
                _logger?.LogWarning(
                    "Failed to modify the state of the local message table to {OptState}, the current State is {State}, Id: {Id}",
                    IntegrationEventStates.Published, eventLog.State, eventLog.Id);
                throw new UserFriendlyException(
                    $"Failed to modify the state of the local message table to {IntegrationEventStates.Published}, the current State is {eventLog.State}, Id: {eventLog.Id}");
            }
        }, cancellationToken);
    }

    public Task MarkEventAsInProgressAsync(Guid eventId, int minimumRetryInterval, CancellationToken cancellationToken = default)
    {
        return UpdateEventStatus(eventId, IntegrationEventStates.InProgress, eventLog =>
        {
            if (eventLog.State is IntegrationEventStates.InProgress or IntegrationEventStates.PublishedFailed &&
                (eventLog.GetCurrentTime() - eventLog.ModificationTime).TotalSeconds < minimumRetryInterval)
            {
                _logger?.LogInformation(
                    "Failed to modify the state of the local message table to {OptState}, the current State is {State}, Id: {Id}, Multitasking execution error, waiting for the next retry",
                    IntegrationEventStates.InProgress, eventLog.State, eventLog.Id);
                throw new UserFriendlyException(
                    $"Failed to modify the state of the local message table to {IntegrationEventStates.InProgress}, the current State is {eventLog.State}, Id: {eventLog.Id}, Multitasking execution error, waiting for the next retry");
            }
            if (eventLog.State != IntegrationEventStates.NotPublished &&
                eventLog.State != IntegrationEventStates.InProgress &&
                eventLog.State != IntegrationEventStates.PublishedFailed)
            {
                _logger?.LogWarning(
                    "Failed to modify the state of the local message table to {OptState}, the current State is {State}, Id: {Id}",
                    IntegrationEventStates.InProgress, eventLog.State, eventLog.Id);
                throw new UserFriendlyException(
                    $"Failed to modify the state of the local message table to {IntegrationEventStates.InProgress}, the current State is {eventLog.State}, Id: {eventLog.Id}");
            }
        }, cancellationToken);
    }

    public Task MarkEventAsFailedAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        return UpdateEventStatus(eventId, IntegrationEventStates.PublishedFailed, eventLog =>
        {
            if (eventLog.State != IntegrationEventStates.InProgress)
            {
                _logger?.LogWarning(
                    "Failed to modify the state of the local message table to {OptState}, the current State is {State}, Id: {Id}",
                    IntegrationEventStates.PublishedFailed, eventLog.State, eventLog.Id);
                throw new UserFriendlyException(
                    $"Failed to modify the state of the local message table to {IntegrationEventStates.PublishedFailed}, the current State is {eventLog.State}, Id: {eventLog.Id}");
            }
        }, cancellationToken);
    }

    public async Task DeleteExpiresAsync(DateTime expiresAt, int batchCount, CancellationToken token = default)
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

    private async Task UpdateEventStatus(Guid eventId,
        IntegrationEventStates status,
        Action<IntegrationEventLog>? action = null,
        CancellationToken cancellationToken = default)
    {
        var eventLogEntry =
            await _eventLogContext.EventLogs.FirstOrDefaultAsync(e => e.EventId == eventId, cancellationToken: cancellationToken);
        if (eventLogEntry == null)
            throw new ArgumentException(
                $"The local message record does not exist, please confirm whether the local message record has been deleted or other reasons cause the local message record to not be inserted successfully In EventId: {eventId}",
                nameof(eventId));

        action?.Invoke(eventLogEntry);

        eventLogEntry.State = status;
        eventLogEntry.ModificationTime = eventLogEntry.GetCurrentTime();

        if (status == IntegrationEventStates.InProgress)
            eventLogEntry.TimesSent++;

        _eventLogContext.EventLogs.Update(eventLogEntry);

        try
        {
            await _eventLogContext.DbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger?.LogWarning(
                ex,
                "Concurrency error, Failed to modify the state of the local message table to {OptState}, the current State is {State}, Id: {Id}",
                status, eventLogEntry.State, eventLogEntry.Id);
            throw new UserFriendlyException("Concurrency conflict, update exception");
        }

        CheckAndDetached(eventLogEntry);
    }

    private void CheckAndDetached(IntegrationEventLog integrationEvent)
    {
        if (_eventLogContext.DbContext.ChangeTracker.QueryTrackingBehavior != QueryTrackingBehavior.TrackAll)
            _eventLogContext.DbContext.Entry(integrationEvent).State = EntityState.Detached;
    }
}
