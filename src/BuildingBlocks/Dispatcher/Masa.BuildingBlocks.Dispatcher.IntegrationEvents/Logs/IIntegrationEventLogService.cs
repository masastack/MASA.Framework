// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Dispatcher.IntegrationEvents.Logs;

public interface IIntegrationEventLogService
{
    /// <summary>
    /// Get messages to retry
    /// </summary>
    /// <param name="retryBatchSize">The size of a single event to be retried</param>
    /// <param name="maxRetryTimes"></param>
    /// <param name="minimumRetryInterval">Minimum retry interval (unit: s)</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IEnumerable<IntegrationEventLog>> RetrieveEventLogsFailedToPublishAsync(
        int retryBatchSize,
        int maxRetryTimes,
        int minimumRetryInterval,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieve pending messages
    /// </summary>
    /// <param name="batchSize">The maximum number of messages retrieved each time</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IEnumerable<IntegrationEventLog>> RetrieveEventLogsPendingToPublishAsync(
        int batchSize,
        CancellationToken cancellationToken = default);

    Task SaveEventAsync(
        IIntegrationEvent @event,
        IntegrationEventExpand? messageExpand,
        DbTransaction transaction,
        CancellationToken cancellationToken = default);

    Task MarkEventAsPublishedAsync(Guid eventId, CancellationToken cancellationToken = default);

    Task<List<Guid>> BulkMarkEventAsPublishedAsync(IEnumerable<Guid> eventIds, CancellationToken cancellationToken = default);

    Task MarkEventAsInProgressAsync(Guid eventId, int minimumRetryInterval, CancellationToken cancellationToken = default);

    Task<List<Guid>> BulkMarkEventAsInProgressAsync(IEnumerable<Guid> eventIds, int minimumRetryInterval, CancellationToken cancellationToken = default);

    Task MarkEventAsFailedAsync(Guid eventId, CancellationToken cancellationToken = default);

    Task<List<Guid>> BulkMarkEventAsFailedAsync(IEnumerable<Guid> eventIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete successfully published and expired data
    /// </summary>
    /// <param name="expiresAt"></param>
    /// <param name="batchCount"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task DeleteExpiresAsync(DateTime expiresAt, int batchCount, CancellationToken token = default);
}
