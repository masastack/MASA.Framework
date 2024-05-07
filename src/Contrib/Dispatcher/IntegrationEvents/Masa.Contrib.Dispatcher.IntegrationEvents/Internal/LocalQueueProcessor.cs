// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Dispatcher.IntegrationEvents;

internal class LocalQueueProcessor
{
    private readonly ConcurrentDictionary<Guid, IntegrationEventLogItem> _retryEventLogs;

    public static ILogger<LocalQueueProcessor>? Logger;
    public static readonly LocalQueueProcessor Default = new();

    public LocalQueueProcessor() => _retryEventLogs = new();

    public static void SetLogger(IServiceCollection services)
    {
        Logger = services.BuildServiceProvider().GetService<ILogger<LocalQueueProcessor>>();
    }

    public void AddJobs(IntegrationEventLogItem items)
        => _retryEventLogs.TryAdd(items.EventId, items);

    public void BulkAddJobs(List<IntegrationEventLogItem> items)
        => items.ForEach(item => _retryEventLogs.TryAdd(item.EventId, item));

    public void RemoveJobs(Guid eventId)
        => _retryEventLogs.TryRemove(eventId, out _);

    public void BulkRemoveJobs(IEnumerable<Guid> eventIds)
        => eventIds.ToList().ForEach(eventId => _retryEventLogs.TryRemove(eventId, out _));

    public void RetryJobs(Guid eventId)
    {
        if (_retryEventLogs.TryGetValue(eventId, out IntegrationEventLogItem? item))
        {
            item.Retry();
        }
    }

    public void BulkRetryJobs(IEnumerable<Guid> eventIds)
    {
        foreach (var eventId in eventIds)
        {
            if (_retryEventLogs.TryGetValue(eventId, out IntegrationEventLogItem? item))
            {
                item.Retry();
            }
        }
    }

    public bool IsExist(Guid eventId)
        => _retryEventLogs.ContainsKey(eventId);

    public List<Guid> IsExist(IEnumerable<Guid> eventIds)
    {
        var notEventIds = new List<Guid>();
        foreach (var eventId in eventIds)
        {
            if (_retryEventLogs.ContainsKey(eventId))
            {
                notEventIds.Add(eventId);
            }
        }

        return notEventIds;
    }

    public void Delete(int maxRetryTimes)
    {
        var eventLogItems = _retryEventLogs.Values.Where(log => log.RetryCount >= maxRetryTimes - 1).ToList();
        eventLogItems.ForEach(item => RemoveJobs(item.EventId));
    }

    public List<IntegrationEventLogItem> RetrieveEventLogsFailedToPublishAsync(int maxRetryTimes, int retryBatchSize)
    {
        try
        {
            return _retryEventLogs
                .Select(item => item.Value)
                .Where(log => log.RetryCount < maxRetryTimes)
                .OrderBy(log => log.RetryCount)
                .ThenBy(log => log.CreationTime)
                .Take(retryBatchSize)
                .ToList();
        }
        catch (Exception ex)
        {
            Logger?.LogWarning(ex, "... getting local retry queue error");

            Thread.Sleep(TimeSpan.FromSeconds(2));
            return new List<IntegrationEventLogItem>();
        }
    }
}
