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

    public Task MarkEventAsInProgressAsync(Guid eventId, CancellationToken cancellationToken = default)
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

    public Task SaveEventAsync(IIntegrationEvent @event, DbTransaction transaction, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
