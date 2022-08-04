// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.EventBus.IntegrationTests.Infrastructure.Extensions;

public class DefaultPublisher : IPublisher
{
    public Task PublishAsync<T>(string topicName, T @event, CancellationToken stoppingToken = default) where T : IIntegrationEvent
    {
        return Task.CompletedTask;
    }
}
