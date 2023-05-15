// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Ddd.Domain.Events;

public abstract record DomainEvent(Guid EventId, DateTime EvenCreateTime) : IDomainEvent
{
    [JsonInclude] public Guid EventId { private get; set; } = EventId;

    [JsonInclude] public DateTime EvenCreateTime { private get; set; } = EvenCreateTime;

    [NotMapped]
    [JsonIgnore]
    public IUnitOfWork? UnitOfWork { get; set; }

    protected DomainEvent() : this(Guid.NewGuid(), DateTime.UtcNow) { }

    public Guid GetEventId() => EventId;

    public void SetEventId(Guid eventId) => EventId = eventId;

    public DateTime GetCreationTime() => EvenCreateTime;

    public void SetCreationTime(DateTime creationTime) => EvenCreateTime = creationTime;
}
