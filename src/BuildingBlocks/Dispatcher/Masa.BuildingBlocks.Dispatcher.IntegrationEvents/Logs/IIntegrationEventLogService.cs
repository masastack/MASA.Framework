// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Dispatcher.IntegrationEvents.Logs;

public interface IIntegrationEventLogService
{
    /// <summary>
    /// Get local messages waiting to be sent
    /// </summary>
    /// <param name="retryBatchSize"></param>
    /// <returns></returns>
    Task<IEnumerable<IntegrationEventLog>> RetrieveEventLogsPendingToPublishAsync(int retryBatchSize = 200);

    /// <summary>
    /// Get message records that need to be retried after sending failures
    /// </summary>
    /// <param name="retryBatchSize">The size of a single event to be retried</param>
    /// <param name="maxRetryTimes"></param>
    /// <param name="minimumRetryInterval">default: 60s</param>
    /// <returns></returns>
    Task<IEnumerable<IntegrationEventLog>> RetrieveEventLogsFailedToPublishAsync(int retryBatchSize = 200, int maxRetryTimes = 10, int minimumRetryInterval = 60);

    Task SaveEventAsync(IIntegrationEvent @event, DbTransaction transaction);

    Task MarkEventAsPublishedAsync(Guid eventId);

    Task MarkEventAsInProgressAsync(Guid eventId);

    Task MarkEventAsFailedAsync(Guid eventId);

    /// <summary>
    /// Delete successfully published and expired data
    /// </summary>
    /// <param name="expiresAt"></param>
    /// <param name="batchCount"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task DeleteExpiresAsync(DateTime expiresAt, int batchCount = 1000, CancellationToken token = default);
}
