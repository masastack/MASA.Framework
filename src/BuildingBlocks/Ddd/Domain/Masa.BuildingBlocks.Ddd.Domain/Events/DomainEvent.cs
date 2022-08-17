// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Ddd.Domain.Events;

public abstract record DomainEvent : IDomainEvent
{
    private Guid _eventId;
    private DateTime _creationTime;

    [JsonIgnore]
    public IUnitOfWork? UnitOfWork { get; set; }

    protected DomainEvent() : this(Guid.NewGuid(), DateTime.UtcNow) { }

    protected DomainEvent(Guid eventId, DateTime creationTime)
    {
        _eventId = eventId;
        _creationTime = creationTime;
    }

    public Guid GetEventId() => _eventId;

    public void SetEventId(Guid eventId) => _eventId = eventId;

    public DateTime GetCreationTime() => _creationTime;

    public void SetCreationTime(DateTime creationTime) => _creationTime = creationTime;
}
