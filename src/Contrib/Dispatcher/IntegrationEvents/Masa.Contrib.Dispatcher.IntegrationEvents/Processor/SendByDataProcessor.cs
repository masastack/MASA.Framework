// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Processor;

public class SendByDataProcessor : ProcessorBase
{
    private readonly IOptions<IntegrationEventOptions> _options;
    private readonly IOptions<MasaAppConfigureOptions>? _masaAppConfigureOptions;
    private readonly ILogger<SendByDataProcessor>? _logger;

    public override int Delay => 1;

    public SendByDataProcessor(
        IServiceProvider serviceProvider,
        IOptions<IntegrationEventOptions> options,
        IOptions<MasaAppConfigureOptions>? masaAppConfigureOptions = null,
        ILogger<SendByDataProcessor>? logger = null) : base(serviceProvider)
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

        var publisher = serviceProvider.GetRequiredService<IPublisher>();
        var eventLogService = serviceProvider.GetRequiredService<IIntegrationEventLogService>();

        var retrieveEventLogs =
            await eventLogService.RetrieveEventLogsPendingToPublishAsync(
                _options.Value.BatchSize,
                stoppingToken);

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
                    eventLog.EventId, _masaAppConfigureOptions?.Value.AppId ?? string.Empty, eventLog);
                await eventLogService.MarkEventAsFailedAsync(eventLog.EventId, stoppingToken);

                LocalQueueProcessor.Default.AddJobs(new IntegrationEventLogItem(eventLog.EventId, eventLog.Topic, eventLog.Event, eventLog.EventExpand));
            }
        }
    }
}
