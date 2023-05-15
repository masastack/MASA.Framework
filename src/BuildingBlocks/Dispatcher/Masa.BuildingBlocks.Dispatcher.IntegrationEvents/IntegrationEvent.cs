// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Dispatcher.IntegrationEvents;

public abstract record IntegrationEvent : IIntegrationEvent
{
    [JsonInclude]public Guid EventId { private get; set; }

    [JsonInclude]
    public DateTime EvenCreateTime { private get; set; }

    [NotMapped] [JsonIgnore] public IUnitOfWork? UnitOfWork { get; set; }

    public virtual string Topic { get; set; }

    protected IntegrationEvent() : this(Guid.NewGuid(), DateTime.UtcNow)
    {
    }

    protected IntegrationEvent(Guid eventId, DateTime creationTime)
    {
        if (string.IsNullOrWhiteSpace(Topic)) Topic = GetType().Name;

        EventId = eventId;
        EvenCreateTime = creationTime;
    }

    public Guid GetEventId() => EventId;

    public void SetEventId(Guid eventId) => EventId = eventId;

    public DateTime GetCreationTime() => EvenCreateTime;

    public void SetCreationTime(DateTime creationTime) => EvenCreateTime = creationTime;
}
