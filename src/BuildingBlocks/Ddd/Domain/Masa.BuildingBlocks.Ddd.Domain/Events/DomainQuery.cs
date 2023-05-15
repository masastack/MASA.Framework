// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Ddd.Domain.Events;

public abstract record DomainQuery<TResult>(Guid EventId, DateTime EvenCreateTime) : IDomainQuery<TResult>
{
    [JsonInclude] public Guid EventId { private get; set; } = EventId;

    [JsonInclude] public DateTime EvenCreateTime { private get; set; } = EvenCreateTime;

    [JsonIgnore]
    public IUnitOfWork? UnitOfWork
    {
        get => null;
        set => throw new NotSupportedException(nameof(UnitOfWork));
    }

    public abstract TResult Result { get; set; }

    protected DomainQuery() : this(Guid.NewGuid(), DateTime.UtcNow) { }

    public Guid GetEventId() => EventId;

    public void SetEventId(Guid eventId) => EventId = eventId;

    public DateTime GetCreationTime() => EvenCreateTime;

    public void SetCreationTime(DateTime creationTime) => EvenCreateTime = creationTime;
}
