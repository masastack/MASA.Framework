// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Dapr;

public class Publisher : IPublisher
{
    public Task PublishAsync<T>(string topicName, T @event, CancellationToken stoppingToken = default) where T : IIntegrationEvent
    {
        throw new NotImplementedException();
    }
}
