// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Dapr;

public class Publisher : IPublisher
{
    private readonly IServiceProvider _serviceProvider;
    private DaprClient? _daprClient;
    public DaprClient DaprClient => _daprClient ??= _serviceProvider.GetRequiredService<DaprClient>();
    private readonly string _pubSubName;
    private readonly string _appId;
    private readonly string? _daprAppId;
    private readonly ILogger<Publisher>? _logger;

    [ExcludeFromCodeCoverage]
    public Publisher(IServiceProvider serviceProvider, string pubSubName, string appId, string? daprAppId)
    {
        _serviceProvider = serviceProvider;
        _logger = serviceProvider.GetService<ILogger<Publisher>>();
        _pubSubName = pubSubName;
        _appId = appId;
        _daprAppId = daprAppId;

        if (serviceProvider.EnableIsolation())
        {
            if (_daprAppId.IsNullOrWhiteSpace())
                _logger?.LogError("Isolation is enabled but dapr AppId required for integration events is not configured");

            MasaArgumentException.ThrowIfNullOrWhiteSpace(daprAppId);
        }
    }

    public async Task PublishAsync<T>(
        string topicName,
        T @event,
        IntegrationEventExpand? eventMessageExpand,
        CancellationToken stoppingToken = default)
    {
        _logger?.LogDebug("----- Integration event publishing is in progress from {AppId} with DaprAppId as '{DaprAppId}'", _appId,
            _daprAppId);

        if (eventMessageExpand is { Isolation.Count: > 0 })
        {
            MasaArgumentException.ThrowIfNullOrWhiteSpace(_daprAppId);

            var eventMessage = new IntegrationEventMessage(@event, eventMessageExpand);

            var masaCloudEvent = new MasaCloudEvent<IntegrationEventMessage>(eventMessage)
            {
                Source = new Uri(_daprAppId, UriKind.RelativeOrAbsolute)
            };
            await DaprClient.PublishEventAsync(_pubSubName, topicName, masaCloudEvent, stoppingToken);
            _logger?.LogDebug(
                "----- Publishing integration event from {AppId} succeeded with DaprAppId is {DaprAppId} and Event is {Event}",
                _appId,
                _daprAppId,
                masaCloudEvent);
        }
        else
        {
            await DaprClient.PublishEventAsync<object>(_pubSubName, topicName, @event!, stoppingToken);
            _logger?.LogDebug(
                "----- Publishing integration event from {AppId} succeeded with DaprAppId is {DaprAppId} and Event is {Event}",
                _appId,
                _daprAppId,
                @event);
        }
    }

    public async Task BulkPublishAsync<T>(
        string topicName, List<(T @event, IntegrationEventExpand? eventMessageExpand)> @events,
        CancellationToken stoppingToken = default)
    {

        _logger?.LogDebug("-----BulkPublishEvent Integration event publishing is in progress from {AppId} with DaprAppId as '{DaprAppId}'", _appId,
            _daprAppId);

        if (!@events.Any())
            return;

        MasaArgumentException.ThrowIfNullOrWhiteSpace(_daprAppId);

        var masaCloudEvents = new List<MasaCloudEvent<IntegrationEventMessage>>();
        var waitEvents = new List<T>();

        @events.ForEach(item =>
        {
            if (item.eventMessageExpand is { Isolation.Count: > 0 })
            {
                var eventMessage = new IntegrationEventMessage(item.@event, item.eventMessageExpand);
                var masaCloudEvent = new MasaCloudEvent<IntegrationEventMessage>(eventMessage)
                {
                    Source = new Uri(_daprAppId, UriKind.RelativeOrAbsolute)
                };

                masaCloudEvents.Add(masaCloudEvent);
            }
            else
            {
                waitEvents.Add(item.@event);
            }
        });

        if (masaCloudEvents.Any())
        {
            await DaprClient.PublishEventAsync(_pubSubName, topicName, masaCloudEvents, stoppingToken);
            _logger?.LogDebug(
                "-----BulkPublishEvent Publishing integration event from {AppId} succeeded with DaprAppId is {DaprAppId} and Event is {Event}",
                _appId,
                _daprAppId,
                masaCloudEvents);
        }

        if (waitEvents.Any())
        {
            await DaprClient.BulkPublishEventAsync(_pubSubName, topicName, @events.ToList(), cancellationToken: stoppingToken);
            _logger?.LogDebug(
                "-----BulkPublishEvent Publishing integration event from {AppId} succeeded with DaprAppId is {DaprAppId} and Event is {Event}",
                _appId,
                _daprAppId,
                @events);
        }
    }
}
