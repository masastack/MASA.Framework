// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents;

public interface IPublisher
{
    Task PublishAsync<T>(
        string topicName,
        T @event,
        IntegrationEventExpand? eventMessageExpand,
        CancellationToken stoppingToken = default);

    Task BulkPublishAsync<T>(
        string topicName, List<(T @event, IntegrationEventExpand? eventMessageExpand)> @events,
        CancellationToken stoppingToken = default);
}
