// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Processor;

/// <summary>
/// Retry sending failed messages
/// </summary>
public class RetryFailedByDataProcessor : ProcessorBase
{
    private readonly IOptions<DispatcherOptions> _options;
    private readonly ILogger<RetryFailedByDataProcessor>? _logger;

    public override int Delay => _options.Value.FailedRetryInterval * 1000;

    public RetryFailedByDataProcessor(
        IServiceProvider serviceProvider,
        IOptions<DispatcherOptions> options,
        ILogger<RetryFailedByDataProcessor>? logger = null) : base(serviceProvider)
    {
        _options = options;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(IServiceProvider serviceProvider, CancellationToken stoppingToken)
    {
        var unitOfWork = serviceProvider.GetService<IUnitOfWork>();
        if (unitOfWork != null) unitOfWork.UseTransaction = false;

        var publisher = serviceProvider.GetRequiredService<IPublisher>();
        var eventLogService = serviceProvider.GetRequiredService<IIntegrationEventLogService>();

        var retrieveEventLogs =
            await eventLogService.RetrieveEventLogsFailedToPublishAsync(_options.Value.RetryBatchSize,
                _options.Value.MaxRetryTimes,
                _options.Value.MinimumRetryInterval);

        string appId = MasaAppConfig.AppId();
        foreach (var eventLog in retrieveEventLogs)
        {
            try
            {
                _logger?.LogDebug("----- Publishing integration event: {IntegrationEventId} from {AppId} - ({@IntegrationEvent})",
                    eventLog.EventId, appId, eventLog.Event);

                if (LocalQueueProcessor.Default.IsExist(eventLog.EventId))
                    continue; // The local queue is retrying, no need to retry

                await eventLogService.MarkEventAsInProgressAsync(eventLog.EventId);

                _logger?.LogDebug("Publishing integration event {EventLog} to {TopicName}",
                    eventLog, eventLog.Event.Topic);

                await publisher.PublishAsync(eventLog.Event.Topic, eventLog.Event, stoppingToken);

                LocalQueueProcessor.Default.TryRemoveJobs(eventLog.EventId);

                await eventLogService.MarkEventAsPublishedAsync(eventLog.EventId);
            }
            catch (UserFriendlyException)
            {
                //Update state due to multitasking contention, no processing required
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex,
                    "Error Publishing integration event: {IntegrationEventId} from {AppId} - ({@IntegrationEvent})",
                    eventLog.EventId, appId, eventLog);
                await eventLogService.MarkEventAsFailedAsync(eventLog.EventId);
            }
        }
    }
}
