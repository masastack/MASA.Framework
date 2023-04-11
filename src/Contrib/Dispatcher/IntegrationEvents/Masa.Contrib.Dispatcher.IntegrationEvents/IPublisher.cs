// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents;

public interface IPublisher
{
    Task PublishAsync<T>(string topicName, T @event, CancellationToken stoppingToken = default) where T : IIntegrationEvent;
    Task PublishAsync<T>(string topicName, T @event, Dictionary<string, string> metadata, CancellationToken stoppingToken = default) where T : IIntegrationEvent;
}
