// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Processor;

public class SendByDataProcessor : ProcessorBase
{
    private readonly IOptions<IntegrationEventOptions> _options;
    private readonly IOptionsMonitor<MasaAppConfigureOptions>? _masaAppConfigureOptions;
    private readonly ILogger<SendByDataProcessor>? _logger;

    public override int Delay => _options.Value.ExecuteInterval;

    public SendByDataProcessor(
        IServiceProvider serviceProvider,
        IOptions<IntegrationEventOptions> options,
        IOptionsMonitor<MasaAppConfigureOptions>? masaAppConfigureOptions = null,
        ILogger<SendByDataProcessor>? logger = null) : base(serviceProvider)
    {
        _masaAppConfigureOptions = masaAppConfigureOptions;
        _options = options;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(IServiceProvider serviceProvider, CancellationToken stoppingToken)
    {
        if (_options.Value.BatchesGroupSendOrRetry)
        {
            await this.BulkExecuteAsync(serviceProvider, stoppingToken);
            return;
        }

        var unitOfWork = serviceProvider.GetService<IUnitOfWork>();
        if (unitOfWork != null)
            unitOfWork.UseTransaction = false;

        var eventLogService = serviceProvider.GetRequiredService<IIntegrationEventLogService>();

        var retrieveEventLogs =
            await eventLogService.RetrieveEventLogsPendingToPublishAsync(
                _options.Value.BatchSize,
                stoppingToken);

        if (!retrieveEventLogs.Any())
            return;

        var publisher = serviceProvider.GetRequiredService<IPublisher>();

        foreach (var eventLog in retrieveEventLogs)
        {
            try
            {
                await eventLogService.MarkEventAsInProgressAsync(eventLog.EventId, _options.Value.MinimumRetryInterval, stoppingToken);

                _logger?.LogDebug("Publishing integration event {Event} to {TopicName}",
                    eventLog,
                    eventLog.Topic);

                await publisher.PublishAsync(eventLog.Topic, eventLog.Event, eventLog.EventExpand, stoppingToken);

                await eventLogService.MarkEventAsPublishedAsync(eventLog.EventId, stoppingToken);
            }
            catch (UserFriendlyException)
            {
                //Update state due to multitasking contention, no processing required
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex,
                    "Error Publishing integration event: {IntegrationEventId} from {AppId} - ({IntegrationEvent})",
                    eventLog.EventId, _masaAppConfigureOptions?.CurrentValue.AppId ?? string.Empty, eventLog);
                await eventLogService.MarkEventAsFailedAsync(eventLog.EventId, stoppingToken);

                LocalQueueProcessor.Default.AddJobs(new IntegrationEventLogItem(eventLog.EventId, eventLog.Topic, eventLog.Event,
                    eventLog.EventExpand));
            }
        }
    }

    protected async Task BulkExecuteAsync(IServiceProvider serviceProvider, CancellationToken stoppingToken)
    {
        var unitOfWork = serviceProvider.GetService<IUnitOfWork>();
        if (unitOfWork != null)
            unitOfWork.UseTransaction = false;

        var eventLogService = serviceProvider.GetRequiredService<IIntegrationEventLogService>();

        var retrieveEventLogs =
            await eventLogService.RetrieveEventLogsPendingToPublishAsync(
                _options.Value.BatchSize,
                stoppingToken);

        if (!retrieveEventLogs.Any())
            return;

        var publisher = serviceProvider.GetRequiredService<IPublisher>();
        var retrieveEventLogsGroupByTopic = retrieveEventLogs.GroupBy(eventLog => eventLog.Topic)
            .Select(eventLog => new
            {
                TopicName = eventLog.Key,
                Events = eventLog.Select(log => new { log.Event, log.EventExpand, log.EventId }).ToList(),
            }).ToList();

        foreach (var eventLog in retrieveEventLogsGroupByTopic)
        {
            var sourceEventIds = eventLog.Events.Select(item => item.EventId);
            var sourceEvents = eventLog.Events;

            try
            {
                var failedEventIds = await eventLogService.BulkMarkEventAsInProgressAsync(sourceEventIds,
                    _options.Value.MinimumRetryInterval, stoppingToken);
                if (failedEventIds.Any())
                {
                    sourceEvents = sourceEvents.Where(item => !failedEventIds.Contains(item.EventId)).ToList();
                    _logger?.LogDebug("Error Publishing integration event {Event} to {TopicName} failedEventIds {failedEventIds}",
                        eventLog, eventLog.TopicName, failedEventIds);
                }
                var eventIds = sourceEvents.Select(item => item.EventId);
                var events = sourceEvents.Select(item => (item.Event, item.EventExpand)).ToList();

                _logger?.LogDebug("Publishing integration event {Event} to {TopicName}",
                    eventLog,
                    eventLog.TopicName);

                await publisher.BulkPublishAsync(eventLog.TopicName, events, stoppingToken);
                await eventLogService.BulkMarkEventAsPublishedAsync(eventIds, stoppingToken);

                if (failedEventIds.Any())
                    await eventLogService.BulkMarkEventAsFailedAsync(failedEventIds, stoppingToken);
            }
            catch (UserFriendlyException)
            {
                //Update state due to multitasking contention, no processing required
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex,
                    "Error Publishing integration event: {IntegrationEventId} from {AppId} - ({IntegrationEvent})",
                    sourceEventIds, _masaAppConfigureOptions?.CurrentValue.AppId ?? string.Empty, eventLog);
                await eventLogService.BulkMarkEventAsFailedAsync(sourceEventIds, stoppingToken);

                var integrationEventLogItem = eventLog.Events.Select(item =>
                    new IntegrationEventLogItem(item.EventId, eventLog.TopicName, item.Event, item.EventExpand)).ToList();

                LocalQueueProcessor.Default.BulkAddJobs(integrationEventLogItem);
            }
        }
    }
}
