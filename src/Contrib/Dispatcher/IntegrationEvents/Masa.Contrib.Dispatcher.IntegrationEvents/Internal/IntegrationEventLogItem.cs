// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Dispatcher.IntegrationEvents;

/// <summary>
/// Use the local queue to retry sending failed messages
/// </summary>
internal class IntegrationEventLogItem
{
    public Guid EventId { get; }

    public string Topic { get; }

    public DateTime CreationTime { get; }

    public int RetryCount { get; private set; }

    public object Event { get; private set; }

    public IntegrationEventExpand? EventExpand { get; private set; }

    public IntegrationEventLogItem(Guid eventId, string topic, object @event, IntegrationEventExpand? eventExpand)
    {
        EventId = eventId;
        Topic = topic;
        RetryCount = 0;
        CreationTime = DateTime.UtcNow;
        Event = @event;
        EventExpand = eventExpand;
    }

    public void Retry()
    {
        this.RetryCount++;
    }
}
