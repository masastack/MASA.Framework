// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Tests.Infrastructure;

public class CustomIntegrationEventLogService : IIntegrationEventLogService
{
    public Task MarkEventAsFailedAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task DeleteExpiresAsync(DateTime expiresAt, int batchCount = 1000, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public Task MarkEventAsInProgressAsync(Guid eventId, int minimumRetryInterval, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task MarkEventAsPublishedAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task<IEnumerable<IntegrationEventLog>> RetrieveEventLogsFailedToPublishAsync(
        int retryBatchSize = 200,
        int maxRetryTimes = 10,
        int minimumRetryInterval = 60,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new List<IntegrationEventLog>().AsEnumerable());
    }

    public Task<IEnumerable<IntegrationEventLog>> RetrieveEventLogsPendingToPublishAsync(int batchSize, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new List<IntegrationEventLog>().AsEnumerable());
    }

    public Task SaveEventAsync(
        IIntegrationEvent @event,
        IntegrationEventExpand? eventExpand,
        DbTransaction transaction,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task BulkMarkEventAsPublishedAsync(IEnumerable<Guid> eventIds, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task BulkMarkEventAsInProgressAsync(IEnumerable<Guid> eventIds, int minimumRetryInterval, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task BulkMarkEventAsFailedAsync(IEnumerable<Guid> eventIds, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
