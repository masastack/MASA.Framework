// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Dapr.Processor;

public class RetryByLocalQueueProcessor : ProcessorBase
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IOptionsMonitor<AppConfig>? _appConfig;
    private readonly IOptions<DispatcherOptions> _options;
    private readonly ILogger<RetryByLocalQueueProcessor>? _logger;

    public override int Delay => _options.Value.LocalFailedRetryInterval;

    public RetryByLocalQueueProcessor(
        IServiceProvider serviceProvider,
        IOptions<DispatcherOptions> options,
        IOptionsMonitor<AppConfig>? appConfig = null,
        ILogger<RetryByLocalQueueProcessor>? logger = null) : base(serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _appConfig = appConfig;
        _options = options;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(IServiceProvider serviceProvider, CancellationToken stoppingToken)
    {
        var unitOfWork = serviceProvider.GetService<IUnitOfWork>();
            if (unitOfWork != null)
                unitOfWork.UseTransaction = false;

            var dapr = serviceProvider.GetRequiredService<DaprClient>();
            var eventLogService = serviceProvider.GetRequiredService<IIntegrationEventLogService>();

            var retrieveEventLogs =
                LocalQueueProcessor.Default.RetrieveEventLogsFailedToPublishAsync(_options.Value.LocalRetryTimes,
                    _options.Value.RetryBatchSize);

            foreach (var eventLog in retrieveEventLogs)
            {
                try
                {
                    LocalQueueProcessor.Default.RetryJobs(eventLog.EventId);

                    await eventLogService.MarkEventAsInProgressAsync(eventLog.EventId);

                    _logger?.LogDebug(
                        "Publishing integration event {Event} to {PubsubName}.{TopicName}",
                        eventLog,
                        _options.Value.PubSubName,
                        eventLog.Topic);

                    await dapr.PublishEventAsync(_options.Value.PubSubName, eventLog.Topic, eventLog.Event, stoppingToken);

                    await eventLogService.MarkEventAsPublishedAsync(eventLog.EventId);

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
                        eventLog.EventId, _appConfig?.CurrentValue.AppId ?? string.Empty, eventLog);
                    await eventLogService.MarkEventAsFailedAsync(eventLog.EventId);
                }
            }
    }
}
