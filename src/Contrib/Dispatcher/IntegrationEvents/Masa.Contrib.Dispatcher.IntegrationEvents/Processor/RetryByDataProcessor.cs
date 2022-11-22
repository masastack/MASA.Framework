// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Processor;

public class RetryByDataProcessor : ProcessorBase
{
    private readonly IOptions<DispatcherOptions> _options;
    private readonly IOptionsMonitor<MasaAppConfigureOptions>? _masaAppConfigureOptions;
    private readonly ILogger<RetryByDataProcessor>? _logger;

    public override int Delay => _options.Value.FailedRetryInterval;

    public RetryByDataProcessor(
        IServiceProvider serviceProvider,
        IOptions<DispatcherOptions> options,
        IOptionsMonitor<MasaAppConfigureOptions>? masaAppConfigureOptions = null,
        ILogger<RetryByDataProcessor>? logger = null) : base(serviceProvider)
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
                await eventLogService.RetrieveEventLogsFailedToPublishAsync(_options.Value.RetryBatchSize, _options.Value.MaxRetryTimes,
                    _options.Value.MinimumRetryInterval, stoppingToken);

            foreach (var eventLog in retrieveEventLogs)
            {
                try
                {
                    if (LocalQueueProcessor.Default.IsExist(eventLog.EventId))
                        continue; // The local queue is retrying, no need to retry

                    await eventLogService.MarkEventAsInProgressAsync(eventLog.EventId, stoppingToken);

                    _logger?.LogDebug("Publishing integration event {Event} to {TopicName}",
                        eventLog,
                        eventLog.Event.Topic);

                    await publisher.PublishAsync(eventLog.Event.Topic, eventLog.Event, stoppingToken);

                    LocalQueueProcessor.Default.RemoveJobs(eventLog.EventId);

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
                }
            }
    }
}
