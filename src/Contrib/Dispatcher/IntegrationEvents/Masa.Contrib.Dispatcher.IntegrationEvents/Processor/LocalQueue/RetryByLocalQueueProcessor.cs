// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Dispatcher.IntegrationEvents;

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

        var publisher = serviceProvider.GetRequiredService<IPublisher>();
        var eventLogService = serviceProvider.GetRequiredService<IIntegrationEventLogService>();

        var retrieveEventLogs = await LocalQueueEventLogService.RetrieveEventLogsToPublishAsync(
            _options.Value.LocalRetryTimes,
            _options.Value.RetryBatchSize);

        foreach (var eventLog in retrieveEventLogs)
        {
            try
            {
                LocalQueueEventLogService.RetryJobs(eventLog.EventId);

                await eventLogService.MarkEventAsInProgressAsync(eventLog.EventId, _options.Value.MinimumRetryInterval, stoppingToken);

                _logger?.LogDebug(
                    "Publishing integration event {Event} to {TopicName}",
                    eventLog,
                    eventLog.Topic);

                await publisher.PublishAsync(eventLog.Topic, eventLog.Event, stoppingToken);

                await eventLogService.MarkEventAsPublishedAsync(eventLog.EventId, stoppingToken);

                LocalQueueEventLogService.RemoveJobs(eventLog.EventId);
            }
            catch (UserFriendlyException)
            {
                //Update state due to multitasking contention
                LocalQueueEventLogService.RemoveJobs(eventLog.EventId);
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
