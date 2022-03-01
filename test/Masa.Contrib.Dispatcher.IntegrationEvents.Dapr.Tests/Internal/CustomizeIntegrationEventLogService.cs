namespace Masa.Contrib.Dispatcher.IntegrationEvents.Dapr.Tests.Internal;

internal class CustomizeIntegrationEventLogService : IIntegrationEventLogService
{
    public Task MarkEventAsFailedAsync(Guid eventId)
    {
        return Task.CompletedTask;
    }

    public Task DeleteExpiresAsync(DateTime expiresAt, int batchCount = 1000, CancellationToken token = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public Task MarkEventAsInProgressAsync(Guid eventId)
    {
        return Task.CompletedTask;
    }

    public Task MarkEventAsPublishedAsync(Guid eventId)
    {
        return Task.CompletedTask;
    }

    public Task<IEnumerable<IntegrationEventLog>> RetrieveEventLogsFailedToPublishAsync(
        int retryBatchSize = 200,
        int maxRetryTimes = 10,
        int minimumRetryInterval = 60)
    {
        return Task.FromResult(new List<IntegrationEventLog>().AsEnumerable());
    }

    public Task SaveEventAsync(IIntegrationEvent @event, DbTransaction transaction)
    {
        return Task.CompletedTask;
    }
}
