// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Dispatcher.Events;

public abstract record Event(Guid EventId, DateTime EvenCreateTime) : IEvent
{
    [JsonInclude] public Guid EventId { private get; set; } = EventId;

    [JsonInclude] public DateTime EvenCreateTime { private get; set; } = EvenCreateTime;

    protected Event() : this(Guid.NewGuid(), DateTime.UtcNow)
    {
    }

    public Guid GetEventId() => EventId;

    public void SetEventId(Guid eventId) => EventId = eventId;

    public DateTime GetCreationTime() => EvenCreateTime;

    public void SetCreationTime(DateTime creationTime) => EvenCreateTime = creationTime;
}
