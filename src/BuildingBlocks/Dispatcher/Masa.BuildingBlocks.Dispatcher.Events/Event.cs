// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Dispatcher.Events;

public abstract record Event(Guid IntegrationEventId, DateTime IntegrationEvenCreateTime) : IEvent
{
    [JsonInclude] public Guid IntegrationEventId { private get; set; } = IntegrationEventId;

    [JsonInclude] public DateTime IntegrationEvenCreateTime { private get; set; } = IntegrationEvenCreateTime;

    protected Event() : this(Guid.NewGuid(), DateTime.UtcNow)
    {
    }

    public Guid GetEventId() => IntegrationEventId;

    public void SetEventId(Guid eventId) => IntegrationEventId = eventId;

    public DateTime GetCreationTime() => IntegrationEvenCreateTime;

    public void SetCreationTime(DateTime creationTime) => IntegrationEvenCreateTime = creationTime;
}
