// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Dapr;

public class Publisher : IPublisher
{
    private readonly IServiceProvider _serviceProvider;
    private DaprClient? _daprClient;
    public DaprClient DaprClient => _daprClient ??= _serviceProvider.GetRequiredService<DaprClient>();
    private readonly string _pubSubName;
    private readonly string? _appId;

    public Publisher(IServiceProvider serviceProvider, string pubSubName, string? appId)
    {
        _serviceProvider = serviceProvider;
        _pubSubName = pubSubName;
        if (serviceProvider.EnableIsolation() && appId == null)
        {
            throw new ArgumentNullException(appId);
        }

        _appId = appId;
    }


    public Task PublishAsync<T>(
        string topicName,
        T @event,
        IntegrationEventExpand? eventMessageExpand,
        CancellationToken stoppingToken = default)
    {
        if (eventMessageExpand is { Isolation.Count: > 0 })
        {
            var integrationEventMessage = new IntegrationEventMessage(@event, eventMessageExpand);

            var masaCloudEvent = new MasaCloudEvent<object>(integrationEventMessage)
            {
                Source = new Uri(_appId, UriKind.RelativeOrAbsolute)
            };
            return DaprClient.PublishEventAsync(_pubSubName, topicName, masaCloudEvent, stoppingToken);
        }

        return DaprClient.PublishEventAsync<object>(_pubSubName, topicName, @event!, stoppingToken);
    }
}
