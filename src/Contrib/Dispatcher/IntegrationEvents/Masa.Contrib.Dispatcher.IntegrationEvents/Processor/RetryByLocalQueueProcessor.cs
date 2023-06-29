// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Processor;

public class RetryByLocalQueueProcessor : ProcessorBase
{
    private readonly IOptionsMonitor<MasaAppConfigureOptions>? _masaAppConfigureOptions;
    private readonly IOptions<IntegrationEventOptions> _options;
    private readonly ILogger<RetryByLocalQueueProcessor>? _logger;

    public override int Delay => _options.Value.LocalFailedRetryInterval;

    public RetryByLocalQueueProcessor(
        IServiceProvider serviceProvider,
        IOptions<IntegrationEventOptions> options,
        IOptionsMonitor<MasaAppConfigureOptions>? masaAppConfigureOptions = null,
        ILogger<RetryByLocalQueueProcessor>? logger = null) : base(serviceProvider)
    {
        _masaAppConfigureOptions = masaAppConfigureOptions;
        _options = options;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(IServiceProvider serviceProvider, CancellationToken stoppingToken)
    {
        var unitOfWork = serviceProvider.GetService<IUnitOfWork>();
        if (unitOfWork != null)
            unitOfWork.UseTransaction = false;

        var eventLogService = serviceProvider.GetRequiredService<IIntegrationEventLogService>();

        var retrieveEventLogs =
            LocalQueueProcessor.Default.RetrieveEventLogsFailedToPublishAsync(_options.Value.LocalRetryTimes,
                _options.Value.RetryBatchSize);

        if(!retrieveEventLogs.Any())
            return;

        var publisher = serviceProvider.GetRequiredService<IPublisher>();

        foreach (var eventLog in retrieveEventLogs)
        {
            try
            {
                LocalQueueProcessor.Default.RetryJobs(eventLog.EventId);

                await eventLogService.MarkEventAsInProgressAsync(eventLog.EventId, _options.Value.MinimumRetryInterval, stoppingToken);

                _logger?.LogDebug(
                    "Publishing integration event {Event} to {TopicName}",
                    eventLog,
                    eventLog.Topic);

                await publisher.PublishAsync(eventLog.Topic, eventLog.Event,  eventLog.EventExpand, stoppingToken);

                await eventLogService.MarkEventAsPublishedAsync(eventLog.EventId, stoppingToken);

                LocalQueueProcessor.Default.RemoveJobs(eventLog.EventId);
            }
            catch (UserFriendlyException)
            {
                //Update state due to multitasking contention
                LocalQueueProcessor.Default.RemoveJobs(eventLog.EventId);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex,
                    "Error Publishing integration event: {IntegrationEventId} from {AppId} - ({IntegrationEvent})",
                    eventLog.EventId, _masaAppConfigureOptions?.CurrentValue.AppId ?? string.Empty, eventLog);
                await eventLogService.MarkEventAsFailedAsync(eventLog.EventId, stoppingToken);
            }
        }
    }
}
