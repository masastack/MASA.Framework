// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Dapr;

public class Publisher : IPublisher
{
    private readonly IServiceProvider _serviceProvider;
    private DaprClient? _daprClient;
    public DaprClient DaprClient => _daprClient ??= _serviceProvider.GetRequiredService<DaprClient>();
    private readonly string _pubSubName;

    public Publisher(IServiceProvider serviceProvider, string pubSubName)
    {
        _serviceProvider = serviceProvider;
        _pubSubName = pubSubName;
    }

    public async Task PublishAsync<T>(string topicName, T @event, CancellationToken stoppingToken = default) where T : IIntegrationEvent
    {
        await DaprClient.PublishEventAsync<object>(_pubSubName, topicName, @event, stoppingToken);
    }

    public async Task PublishAsync<T>(string topicName, T @event, Dictionary<string, string> metadata, CancellationToken stoppingToken = default) where T : IIntegrationEvent
    {
        await DaprClient.PublishEventAsync<object>(_pubSubName, topicName, @event, metadata, stoppingToken);
    }
}
