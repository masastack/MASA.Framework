// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Dapr.Processor;

public class RetryByDataProcessor : ProcessorBase
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IOptions<DispatcherOptions> _options;
    private readonly IOptionsMonitor<AppConfig>? _appConfig;
    private readonly ILogger<RetryByDataProcessor>? _logger;

    public override int Delay => _options.Value.FailedRetryInterval;

    public RetryByDataProcessor(
        IServiceProvider serviceProvider,
        IOptions<DispatcherOptions> options,
        IOptionsMonitor<AppConfig>? appConfig = null,
        ILogger<RetryByDataProcessor>? logger = null) : base(serviceProvider)
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
                await eventLogService.RetrieveEventLogsFailedToPublishAsync(_options.Value.RetryBatchSize, _options.Value.MaxRetryTimes,
                    _options.Value.MinimumRetryInterval);

            foreach (var eventLog in retrieveEventLogs)
            {
                try
                {
                    if (LocalQueueProcessor.Default.IsExist(eventLog.EventId))
                        continue; // The local queue is retrying, no need to retry

                    await eventLogService.MarkEventAsInProgressAsync(eventLog.EventId);

                    _logger?.LogDebug("Publishing integration event {Event} to {PubsubName}.{TopicName}", eventLog,
                        _options.Value.PubSubName,
                        eventLog.Event.Topic);

                    await dapr.PublishEventAsync(_options.Value.PubSubName, eventLog.Event.Topic, eventLog.Event, stoppingToken);

                    LocalQueueProcessor.Default.RemoveJobs(eventLog.EventId);

                    await eventLogService.MarkEventAsPublishedAsync(eventLog.EventId);
                }
                catch (UserFriendlyException)
                {
                    //Update state due to multitasking contention, no processing required
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
