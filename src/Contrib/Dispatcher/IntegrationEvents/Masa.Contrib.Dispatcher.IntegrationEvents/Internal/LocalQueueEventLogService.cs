// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Internal;

internal class LocalQueueEventLogService
{
    private readonly ILogger<LocalQueueEventLogService>? _logger;
    /// <summary>
    ///
    /// </summary>
    private readonly ConcurrentDictionary<Guid, IntegrationEventLogItem> _retryEventLogs;
    private readonly TimeSpan _delayTimeSpan;

    public LocalQueueEventLogService(ILogger<LocalQueueEventLogService>? logger)
    {
        _logger = logger;
        _retryEventLogs = new ConcurrentDictionary<Guid, IntegrationEventLogItem>();
        _delayTimeSpan = TimeSpan.FromSeconds(2);
    }

    public void AddJobs(IntegrationEventLogItem items)
        => _retryEventLogs.TryAdd(items.EventId, items);

    public void RemoveJobs(Guid eventId)
        => _retryEventLogs.TryRemove(eventId, out _);

    public void RetryJobs(Guid eventId)
    {
        if (_retryEventLogs.TryGetValue(eventId, out IntegrationEventLogItem? item))
        {
            item.Retry();
        }
    }

    public bool IsExist(Guid eventId)
        => _retryEventLogs.ContainsKey(eventId);

    public void Delete(int maxRetryTimes)
    {
        var eventLogItems = _retryEventLogs.Values.Where(log => log.RetryCount >= maxRetryTimes - 1).ToList();
        eventLogItems.ForEach(item => RemoveJobs(item.EventId));
    }

    public async Task<List<IntegrationEventLogItem>> RetrieveEventLogsToPublishAsync(int maxRetryTimes, int retryBatchSize)
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
            _logger?.LogWarning(ex, "... Get the queue error of the local message to be sent");

            await Task.Delay(_delayTimeSpan);
            return new List<IntegrationEventLogItem>();
        }
    }
}
