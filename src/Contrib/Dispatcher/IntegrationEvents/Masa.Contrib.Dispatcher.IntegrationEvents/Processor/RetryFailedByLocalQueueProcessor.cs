// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Processor;

public class RetryFailedByLocalQueueProcessor : ProcessorBase
{
    private readonly IOptions<DispatcherOptions> _options;
    private readonly ILogger<RetryFailedByLocalQueueProcessor>? _logger;

    public override int Delay => _options.Value.LocalFailedRetryInterval * 1000;

    public RetryFailedByLocalQueueProcessor(
        IServiceProvider serviceProvider,
        IOptions<DispatcherOptions> options,
        ILogger<RetryFailedByLocalQueueProcessor>? logger = null) : base(serviceProvider)
    {
        _options = options;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(IServiceProvider serviceProvider, CancellationToken stoppingToken)
    {
        var unitOfWork = serviceProvider.GetService<IUnitOfWork>();
        if (unitOfWork != null)
            unitOfWork.UseTransaction = false;

        var publisher = serviceProvider.GetRequiredService<IPublisher>();
        var eventLogService = serviceProvider.GetRequiredService<IIntegrationEventLogService>();

        var retrieveEventLogs =
            LocalQueueProcessor.Default.RetrieveEventLogsFailedToPublishAsync(_options.Value.LocalRetryTimes,
                _options.Value.RetryBatchSize);

        string appId = MasaAppConfig.AppId();
        foreach (var eventLog in retrieveEventLogs)
        {
            try
            {
                _logger?.LogDebug("----- Publishing integration event: {IntegrationEventId} from {AppId} - ({@IntegrationEvent})",
                    eventLog.EventId, appId, eventLog.Event);

                LocalQueueProcessor.Default.TryRetryJobs(eventLog.EventId);

                await eventLogService.MarkEventAsInProgressAsync(eventLog.EventId);

                _logger?.LogDebug(
                    "Publishing integration event {EventLog} to {TopicName}",
                    eventLog, eventLog.Topic);

                await publisher.PublishAsync(eventLog.Topic, eventLog.Event, stoppingToken);

                await eventLogService.MarkEventAsPublishedAsync(eventLog.EventId);

                LocalQueueProcessor.Default.TryRemoveJobs(eventLog.EventId);
            }
            catch (UserFriendlyException)
            {
                //Update state due to multitasking contention
                LocalQueueProcessor.Default.TryRemoveJobs(eventLog.EventId);
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
