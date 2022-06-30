// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Dapr;

public class Publisher : IPublisher
{
    private readonly IServiceProvider _serviceProvider;
    private DaprClient? _daprClient;
    public DaprClient DaprClient => _daprClient ??= _serviceProvider.GetRequiredService<DaprClient>();
    private readonly DispatcherOptions _dispatcherOptions;

    public Publisher(IServiceProvider serviceProvider, DispatcherOptions dispatcherOptions)
    {
        _serviceProvider = serviceProvider;
        _dispatcherOptions = dispatcherOptions;
    }

    public async Task PublishAsync<T>(string topicName, T @event, CancellationToken stoppingToken = default) where T : IIntegrationEvent
    {
        await DaprClient.PublishEventAsync(_dispatcherOptions.PubSubName, topicName, @event, stoppingToken);
    }
}
